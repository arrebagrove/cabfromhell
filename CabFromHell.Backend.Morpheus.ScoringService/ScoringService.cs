using CabFromHell.Backend.Morpheus.ScoringService.Interfaces;
using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using System.Text;

namespace CabFromHell.Backend.Morpheus.ScoringService
{
    /// <remarks>
    /// Each ActorID maps to an instance of this class.
    /// The IProjName  interface (in a separate DLL that client code can
    /// reference) defines the operations exposed by ProjName objects.
    /// </remarks>
    internal class CabDriver : StatefulActor<CabDriver.CabDriverState>, ICabDriver
    {
        static string eventHubName = "cabfromhell";
        static string connectionString = "Endpoint=sb://cabfromhell.servicebus.windows.net/;SharedAccessKeyName=hellcluster;SharedAccessKey=YVlPE3nu5UneW/5fWvJ+CjywrYBGz+C886QdughEMa4=";

        /// <summary>
        /// This class contains each actor's replicated state.
        /// Each instance of this class is serialized and replicated every time an actor's state is saved.
        /// For more information, see http://aka.ms/servicefabricactorsstateserialization
        /// </summary>
        [DataContract]
        internal sealed class CabDriverState
        {
            [DataMember]
            public string Name { get; set; }

            [DataMember]
            public double Score { get; set; }

            [DataMember]
            public long ScoreUpdates { get; set; }

            [DataMember]
            public DateTime LastUpdated { get; set; }

            public override string ToString()
            {
                return $"{Name}|{Score}|{ScoreUpdates}|{LastUpdated}";
            }
        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// </summary>
        protected override Task OnActivateAsync()
        {
            if (this.State == null)
            {
                // This is the first time this actor has ever been activated.
                // Set the actor's initial state values.
                this.State = new CabDriverState { Name = "Jon Doe", Score = 0d, ScoreUpdates = 0, LastUpdated = DateTime.UtcNow};
            }

            ActorEventSource.Current.ActorMessage(this, "State initialized to {0}", this.State);
            return Task.FromResult(true);
        }

        public Task UpdateScoreAsync(DateTime arrivalDate, double forceValue, int ammountOfMeasurements, double timeInterval)
        {
            ActorEventSource.Current.ActorMessage(this, $"Uppdating Score from {this.State.Score} for {this.State.Name} with values: forceValue = {forceValue}, measurements = {ammountOfMeasurements}, timeInterval = {timeInterval}");

            var currentScore = forceValue / ammountOfMeasurements;

            this.State.Score = (this.State.Score * this.State.ScoreUpdates + currentScore) / (this.State.ScoreUpdates + 1);

            SendScoreToEventHub(this.State.Name, this.State.Score, this.State.LastUpdated);

            ActorEventSource.Current.ActorMessage(this, $"Uppdating Score to {this.State.Score} for {this.State.Name}");

            return Task.FromResult(true);
        }

        public Task SetNameAsync(string name)
        {
            ActorEventSource.Current.ActorMessage(this, $"Setting Name to {name}");
            this.State.Name = name;
            return Task.FromResult(true);
        }

        [Readonly]
        public Task<string> GetNameAsync()
        {
            ActorEventSource.Current.ActorMessage(this, $"Getting Name as {this.State.Name}");
            return Task.FromResult(this.State.Name);
        }

        [Readonly]
        public Task<double> GetScoreAsync()
        {
            ActorEventSource.Current.ActorMessage(this, $"Getting Score as {this.State.Score}");
            return Task.FromResult(this.State.Score);
        }

        [Readonly]
        public Task<long> GetScoreUpdatesAsync()
        {
            ActorEventSource.Current.ActorMessage(this, $"Getting ScoreUpdates as {this.State.ScoreUpdates}");
            return Task.FromResult(this.State.ScoreUpdates);
        }

        public Task<DateTime> GetLastUpdatedAsync()
        {
            ActorEventSource.Current.ActorMessage(this, $"Getting LastUpdated as {this.State.LastUpdated}");
            return Task.FromResult(this.State.LastUpdated);
        }

        //[Readonly]
        //Task<int> ICabDriver.GetMeasurementCountAsync()
        //{
        //    // For methods that do not change the actor's state,
        //    // [Readonly] improves performance by not performing serialization and replication of the actor's state.
        //    ActorEventSource.Current.ActorMessage(this, "Getting current count value as {0}", this.State.Count);
        //    return Task.FromResult(this.State.Count);
        //}

        //Task IScoringService.SetCountAsync(int count)
        //{
        //    ActorEventSource.Current.ActorMessage(this, "Setting current count of value to {0}", count);
        //    this.State.Count = count;  // Update the state

        //    return Task.FromResult(true);
        //    // When this method returns, the Actor framework automatically
        //    // serializes & replicates the actor's state.
        //}

        private void SendScoreToEventHub(string name, double score, DateTime lastUpdated)
        {
            var msg = string.Format(CultureInfo.InvariantCulture, "{0}|{1}|{2}", name, score, lastUpdated);

            var eventHubClient = EventHubClient.CreateFromConnectionString(connectionString, eventHubName);

            try
            {
                ActorEventSource.Current.ActorMessage(this, $"Sending message to EventHub: {msg}");

                eventHubClient.Send(new EventData(Encoding.UTF8.GetBytes(msg)));

                ActorEventSource.Current.ActorMessage(this, "Message sent to EventHub");
            }
            catch (Exception exception)
            {
                ActorEventSource.Current.ActorMessage(this, "ERROR sending to EventHub: {0}", exception.Message);
            }
        }
    }
}
