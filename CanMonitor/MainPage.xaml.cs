using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Azure.Devices.Client;
using GIS = GHIElectronics.UWP.Shields;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CanMonitor
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // IOT Hub part
        private const string DeviceConnectionString = "HostName=hubtohellandback.azure-devices.net;DeviceId=CrazyCabA;SharedAccessKey=heZ9orRd7tcq7Mv5vXnlyDXMjc71Q2Xh7SPueBVSV/0=";
        private DeviceClient deviceClient;

        // Sensor access part
        private GIS.FEZHAT hat;
        private DispatcherTimer timer;
        private bool next;

        // Algorithm part
        private double _x, _y, _z;
        double _aggregatedLength;
        private int counter;

        public MainPage()
        {
            this.InitializeComponent();
            Setup();
        }

        /// <summary>
        /// Setup for IOT Hub connection and sensor access
        /// </summary>
        private async void Setup()
        {
            // IOT Hub
            deviceClient = DeviceClient.CreateFromConnectionString(DeviceConnectionString, TransportType.Http1);

            // Sensors
            this.hat = await GIS.FEZHAT.CreateAsync();

            // Timer to read sensors
            this.timer = new DispatcherTimer();
            this.timer.Interval = TimeSpan.FromMilliseconds(100);
            this.timer.Tick += this.OnTick;
            this.timer.Start();

        }

        private async void OnTick(object sender, object e)
        {
            // Get acceleration and calculate delta for previous run
            double x, y, z, deltaX, deltaY, deltaZ;
            this.hat.GetAcceleration(out x, out y, out z);
            deltaX = x - _x;
            deltaY = y - _y;
            deltaZ = z + _z;

            _x = x;
            _y = y;
            _z = z;

            _aggregatedLength += Math.Sqrt((deltaX * deltaX) + (deltaY * deltaY) + (deltaZ * deltaZ));

            // Send every 5 seconds data to IOT Hub
            if (counter >= 50)
            {
                try
                {
                    string dataBuffer = $"Boogieman|{_aggregatedLength:N4}|{counter / 10}|{counter}";
                    _aggregatedLength = 0;
                    counter = 0;

                    Message eventMessage = new Message(Encoding.UTF8.GetBytes(dataBuffer));
                    Debug.WriteLine(dataBuffer);
                    await deviceClient.SendEventAsync(eventMessage);
                }
                catch (Exception)
                {
                    Debug.WriteLine("Can't send message because of network issues");
                }
                // Just for fun, do police car lights to Fezhat leds
                this.hat.D2.Color = this.next ? GIS.FEZHAT.Color.Blue : GIS.FEZHAT.Color.Red;
                this.hat.D3.Color = this.next ? GIS.FEZHAT.Color.Red : GIS.FEZHAT.Color.Blue;

                this.next = !this.next;
            }
            counter++;
        }

        /// <summary>
        /// Not used for now, but if there's need for receiving data from IOT HUB this is needed
        /// </summary>
        /// <param name="deviceClient"></param>
        /// <returns></returns>
        static async Task ReceiveCommands(DeviceClient deviceClient)
        {
            Debug.WriteLine("\nDevice waiting for commands from IoTHub...\n");
            Message receivedMessage;
            string messageData;

            while (true)
            {
                receivedMessage = await deviceClient.ReceiveAsync();

                if (receivedMessage != null)
                {
                    messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                    Debug.WriteLine("\t{0}> Received message: {1}", DateTime.Now.ToLocalTime(), messageData);

                    await deviceClient.CompleteAsync(receivedMessage);
                }

                await Task.Delay(TimeSpan.FromSeconds(10));
            }
        }
    }
}
