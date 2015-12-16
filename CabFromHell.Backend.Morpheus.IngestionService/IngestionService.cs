using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.ServiceBus.Messaging;
using System.Text;

namespace CabFromHell.Backend.Morpheus.IngestionService
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance.
    /// </summary>
    internal sealed class IngestionService : StatefulService
    {
        static string connectionString = "HostName=hubtohellandback.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=xL2vntD8upPVocC6Cmayd510RtnVzUfjdf88vcjNbJE=";
        static string iotHubD2cEndpoint = "messages/events";
        static EventHubClient eventHubClient;
        static IReliableDictionary<string, string> cabDriverDictionary;

        /// <summary>
        /// Optional override to create listeners (like tcp, http) for this service replica.
        /// </summary>
        /// <returns>The collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            // TODO: If your service needs to handle user requests, return a list of ServiceReplicaListeners here.
            return new ServiceReplicaListener[0];
        }

        /// <summary>
        /// This is the main entry point for your service's partition replica. 
        /// RunAsync executes when the primary replica for this partition has write status.
        /// </summary>
        /// <param name="cancelServicePartitionReplica">Canceled when Service Fabric terminates this partition's replica.</param>
        protected override async Task RunAsync(CancellationToken cancelServicePartitionReplica)
        {
            // Gets (or creates) a replicated dictionary called "myDictionary" in this partition.
            cabDriverDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, string>>("cabDrivers");

            // This partition's replica continues processing until the replica is terminated.
            while (!cancelServicePartitionReplica.IsCancellationRequested)
            {

                Console.WriteLine("Receive messages\n");
                eventHubClient = EventHubClient.CreateFromConnectionString(connectionString, iotHubD2cEndpoint);

                var d2cPartitions = eventHubClient.GetRuntimeInformation().PartitionIds;

                foreach (string partition in d2cPartitions)
                {
                    ReceiveMessagesFromDeviceAsync(partition, cancelServicePartitionReplica);
                }

                // Pause for 1 second before continue processing.
                await Task.Delay(TimeSpan.FromSeconds(1), cancelServicePartitionReplica);
            }
        }

        private async Task ReceiveMessagesFromDeviceAsync(string partition, CancellationToken cancelServicePartitionReplica)
        {
            var eventHubReceiver = eventHubClient.GetDefaultConsumerGroup().CreateReceiver(partition, DateTime.Now);
            while (!cancelServicePartitionReplica.IsCancellationRequested)
            {
                EventData eventData = await eventHubReceiver.ReceiveAsync();
                if (eventData == null) continue;

                string data = Encoding.UTF8.GetString(eventData.GetBytes());
                Console.WriteLine(string.Format("Message received. Partition: {0} Data: '{1}'", partition, data));

                // Create a transaction to perform operations on data within this partition's replica.
                using (var tx = this.StateManager.CreateTransaction())
                {

                    // Try to read a value from the dictionary whose key is "Counter-1".
                    var result = await cabDriverDictionary.TryGetValueAsync(tx, "Counter-1");

                    // Log whether the value existed or not.
                    ServiceEventSource.Current.ServiceMessage(this, "Current Counter Value: {0}",
                        result.HasValue ? result.Value.ToString() : "Value does not exist.");

                    // If the "Counter-1" key doesn't exist, set its value to 0
                    // else add 1 to its current value.
                   // await cabDriverDictionary.AddOrUpdateAsync(tx, "Counter-1", 0, (k, v) => ++v);

                    // Committing the transaction serializes the changes and writes them to this partition's secondary replicas.
                    // If an exception is thrown before calling CommitAsync, the transaction aborts, all changes are 
                    // discarded, and nothing is sent to this partition's secondary replicas.
                    await tx.CommitAsync();
                }

            }
        }
    }
}
