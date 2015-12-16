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
        private static int MESSAGE_COUNT = 5;
        private const string DeviceConnectionString = "HostName=hubtohellandback.azure-devices.net;DeviceId=CrazyCabA;SharedAccessKey=heZ9orRd7tcq7Mv5vXnlyDXMjc71Q2Xh7SPueBVSV/0=";

        private GIS.FEZHAT hat;
        private DispatcherTimer timer;
        private bool next;
        private int i;
        private DeviceClient deviceClient;

        public MainPage()
        {
            this.InitializeComponent();
            Setup();
            //Start();
        }

        private async void Setup()
        {
            deviceClient = DeviceClient.CreateFromConnectionString(DeviceConnectionString, TransportType.Http1);

            this.hat = await GIS.FEZHAT.CreateAsync();

            this.timer = new DispatcherTimer();
            this.timer.Interval = TimeSpan.FromMilliseconds(1000);
            this.timer.Tick += this.OnTick;
            this.timer.Start();

        }

        private async void OnTick(object sender, object e)
        {
            double x, y, z;

            this.hat.GetAcceleration(out x, out y, out z);
            this.LightTextBox.Text = this.hat.GetLightLevel().ToString("P2");
            this.TempTextBox.Text = this.hat.GetTemperature().ToString("N2");
            this.AccelTextBox.Text = $"({x:N2}, {y:N2}, {z:N2})";
            System.Diagnostics.Debug.WriteLine($"({x}, {y}, {z})");

            try
            {
                string dataBuffer = $"({x:N2}, {y:N2}, {z:N2})";
                Message eventMessage = new Message(Encoding.UTF8.GetBytes(dataBuffer));

                await deviceClient.SendEventAsync(eventMessage);
            }
            catch (Exception)
            {
                Debug.WriteLine("Can't send message because of network issues");
            }

            if ((this.i++%5) == 0)
            {
                this.LedsTextBox.Text = this.next.ToString();
                
                this.hat.D2.Color = this.next ? GIS.FEZHAT.Color.Blue : GIS.FEZHAT.Color.Red;
                this.hat.D3.Color = this.next ? GIS.FEZHAT.Color.Red : GIS.FEZHAT.Color.Blue;
                
                this.next = !this.next;
            }

        }

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
