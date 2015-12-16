using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using GIS = GHIElectronics.UWP.Shields;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CanMonitor
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private GIS.FEZHAT hat;
        private DispatcherTimer timer;
        private bool next;
        private int i;

        public MainPage()
        {
            this.InitializeComponent();
            Setup();
        }

        private async void Setup()
        {
            this.hat = await GIS.FEZHAT.CreateAsync();

            this.timer = new DispatcherTimer();
            this.timer.Interval = TimeSpan.FromMilliseconds(100);
            this.timer.Tick += this.OnTick;
            this.timer.Start();
        }

        private void OnTick(object sender, object e)
        {
            double x, y, z;

            this.hat.GetAcceleration(out x, out y, out z);
            this.LightTextBox.Text = this.hat.GetLightLevel().ToString("P2");
            this.TempTextBox.Text = this.hat.GetTemperature().ToString("N2");
            this.AccelTextBox.Text = $"({x:N2}, {y:N2}, {z:N2})";
            System.Diagnostics.Debug.WriteLine($"({x}, {y}, {z})");

            if ((this.i++%5) == 0)
            {
                this.LedsTextBox.Text = this.next.ToString();
                
                this.hat.D2.Color = this.next ? GIS.FEZHAT.Color.Blue : GIS.FEZHAT.Color.Red;
                this.hat.D3.Color = this.next ? GIS.FEZHAT.Color.Red : GIS.FEZHAT.Color.Blue;
                
                this.next = !this.next;
            }

        }
    }
}
