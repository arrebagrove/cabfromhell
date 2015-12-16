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
        static RegistryManager registryManager;
        static string connectionString = "HostName=hubtohellandback.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=xL2vntD8upPVocC6Cmayd510RtnVzUfjdf88vcjNbJE=";

        static DeviceClient deviceClient;
        static string iotHubUri = "hubtohellandback.azure-devices.net";
        static string deviceKey = "Xrlz595lzPHsPxWdVmNGG6iC9IZwKTjMuBuA/UGQkrQ=";

        static void Main(string[] args)
        {
            //// REGISTER
            //registryManager = RegistryManager.CreateFromConnectionString(connectionString);
            //AddDeviceAsync().Wait();
            //Console.ReadLine();

            // SEND
            Console.WriteLine("Simulated device\n");
            deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey("myFirstDevice", deviceKey));

            SendDeviceToCloudMessagesAsync();
            Console.ReadLine();
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
            double avgWindSpeed = 10; // m/s
            Random rand = new Random();

            while (true)
            {
                var taxiDriver = "Ola Olsson";
                var forceVectorLength = 3.1415d;
                var measurements = 50;
                var timeInterval = 5.12d;

                var msg = string.Format(CultureInfo.InvariantCulture,
                    "{0}|{1}|{2}|{3}", taxiDriver, forceVectorLength, measurements, timeInterval);

                var message = new Microsoft.Azure.Devices.Client.Message(Encoding.ASCII.GetBytes(msg));

                await deviceClient.SendEventAsync(message);
                Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, msg);

                Thread.Sleep(1000);
            }
        }

    }
}
