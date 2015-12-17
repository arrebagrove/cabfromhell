using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System.Threading;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using System.Globalization;

namespace IoTHubDevice
{
    class Program
    {
        static string eventHubName = "cabdriverscores";
        static string serviceBusconnectionString =
            "Endpoint=sb://cabfromhell.servicebus.windows.net/;SharedAccessKeyName=hellcluster;SharedAccessKey=YVlPE3nu5UneW/5fWvJ+CjywrYBGz+C886QdughEMa4=";

        static RegistryManager registryManager;
        static string connectionString = "HostName=hubtohellandback.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=xL2vntD8upPVocC6Cmayd510RtnVzUfjdf88vcjNbJE=";

        static DeviceClient deviceClient;
        static string iotHubUri = "hubtohellandback.azure-devices.net";
        static string deviceKey = "Xrlz595lzPHsPxWdVmNGG6iC9IZwKTjMuBuA/UGQkrQ=";

        static void Main(string[] args)
        {
            Console.WriteLine("1. Register Device");
            Console.WriteLine("2. Send Fake Device Data");
            Console.WriteLine("3. Send Fake Data to Output EventHub");

            var choice = Console.ReadLine();

            if (choice == "1")
            {
                registryManager = RegistryManager.CreateFromConnectionString(connectionString);
                AddDeviceAsync().Wait();
                Console.ReadLine();
            }
            else if (choice == "2")
            {
                Console.WriteLine("Simulated device\n");
                deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey("myFirstDevice", deviceKey), Microsoft.Azure.Devices.Client.TransportType.Http1);

                SendDeviceToCloudMessagesAsync();
                Console.ReadLine();
            }
            else
            {
                SendScoreToEventHub();
            }

            //// SERVICE BUS TEST
            //SendScoreToEventHub();

            ////Console.ReadLine();

            ////// REGISTER
            //registryManager = RegistryManager.CreateFromConnectionString(connectionString);
            //AddDeviceAsync().Wait();
            //Console.ReadLine();

            ////// SEND
            //Console.WriteLine("Simulated device\n");
            //deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey("myFirstDevice", deviceKey), Microsoft.Azure.Devices.Client.TransportType.Http1);

            //SendDeviceToCloudMessagesAsync();
            //Console.ReadLine();
        }

        private async static Task AddDeviceAsync()
        {
            string deviceId = "myFirstDevice";
            Device device;
            try
            {
                device = await registryManager.AddDeviceAsync(new Device(deviceId));
            }
            catch (DeviceAlreadyExistsException)
            {
                device = await registryManager.GetDeviceAsync(deviceId);
            }
            Console.WriteLine("Generated device key: {0}", device.Authentication.SymmetricKey.PrimaryKey);
        }

        private static async void SendDeviceToCloudMessagesAsync()
        {
            Random rand = new Random();

            while (true)
            {
                var taxiDriver = "Ola Olsson";
                var forceVectorLength = rand.NextDouble() * 10;
                var timeInterval = 5.12d;
                var measurements = 50;

                var msg = string.Format(CultureInfo.InvariantCulture,
                    "{0}|{1}|{2}|{3}", taxiDriver, forceVectorLength, timeInterval, measurements);

                var message = new Microsoft.Azure.Devices.Client.Message(Encoding.ASCII.GetBytes(msg));

                await deviceClient.SendEventAsync(message);
                Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, msg);

                Thread.Sleep(1000);
            }
        }

        private static void SendScoreToEventHub()
        {
            var rand = new Random();
            var names = new[] { "Kristofer", "Valery", "Einar", "Jani" };
            while (true)
            {
                var name = names[rand.Next(names.Length)];
                var score = rand.NextDouble() * 5;
                var lastUpdated = DateTime.UtcNow;

                var msg = string.Format(CultureInfo.InvariantCulture, "{{ 'name': '{0}', 'score': {1}, 'lastUpdated': '{2}' }}", name, score, lastUpdated);

                Console.WriteLine(msg);

                var eventHubClient = EventHubClient.CreateFromConnectionString(serviceBusconnectionString, eventHubName);

                try
                {
                    //ActorEventSource.Current.ActorMessage(this, $"Sending message to EventHub: {msg}");

                    eventHubClient.Send(new EventData(Encoding.UTF8.GetBytes(msg)));

                    //ActorEventSource.Current.ActorMessage(this, "Message sent to EventHub");
                }
                catch (Exception exception)
                {
                    //ActorEventSource.Current.ActorMessage(this, "ERROR sending to EventHub: {0}", exception.Message);
                }
            }
        }

    }
}
