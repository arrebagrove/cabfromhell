using System;
using System.Collections.Generic;

using Xamarin.Forms;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using Autofac;

namespace CabFromHell.Mobile
{
	public partial class MainPage : ContentPage
	{
		double _aggregatedLength;
		DateTime _lastSend;


		public MainPage ()
		{
			InitializeComponent ();

			var accellerometer = App.Container.Resolve<IAccellerometer> ();


			_lastSend = DateTime.Now;

			accellerometer.AddHandler (async (x, y, z) => {

				//sqrt(x^2 + y^2 + z^2)

				var length = Math.Sqrt((x*x)+(y*y)+(z*z));
				_aggregatedLength += length;

				var now = DateTime.Now;
				var delta = now.Subtract(_lastSend);
				if( delta.TotalSeconds >= 5 ) {
					var aggregatedLength = _aggregatedLength;
					_aggregatedLength = 0;

					await SendEntry("HorseManeuerMan", aggregatedLength, delta.TotalSeconds);

					_lastSend = now;
				}
			});
		}


		void ButtonClicked(object sender, EventArgs e) 
		{
			var i = 0;
			i++;
		}





		async Task SendEntry(string source, double length, double totalSeconds)
		{

			var buffer = $"{source}|{length:N4}|{totalSeconds:N4}";
			//var buffer = $"{source}|{x:N2}|{y:N2}|{z:N2}";




			/*
POST /devices/CrazyCabA/messages/events?api-version=2015-08-15-preview HTTP/1.1
 
POST https://hubtohellandback.azure-devices.net/devices/CrazyCabA/messages/events?api-version=2015-08-15-preview HTTP/1.1
Accept: application/json
Authorization: SharedAccessSignature sr=hubtohellandback.azure-devices.net%2Fdevices%2FCrazyCabA&sig=1dLTN4feSpJhzHQ1QtdxYKOTc5Z%2BchZJ8qmr0Za1m4Q%3D&se=1450280372
Content-Length: 18
Host: hubtohellandback.azure-devices.net
Connection: Keep-Alive
  
 * */
			var serviceNamespace = "hubtohellandback";
			var host = "azure-devices.net";
			var device = "CrazyCabB";
			var key = "gENd9WJ6otc2sEWXuS/CRwqxWjlqSY5TmodJOjF/Vgs=";
			var ttl = "1450280372";

			var sas = $"SharedAccessSignature sr={serviceNamespace}.{host}%2Fdevices%2F{device}&sig={key}"; // &se={ttl}

			var action = $"devices/{device}/messages/events?api-version=2015-08-15-preview";

			var httpClient = new HttpClient
			{
				BaseAddress = new Uri($"https://{serviceNamespace}.{host}")
			};

			httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", sas);
			var content = new StringContent(buffer, Encoding.UTF8, "application/json");

			var contentType = "application/json;type=entry;charset=utf-8";
			content.Headers.Add("ContentType", contentType);
			var response = await httpClient.PostAsync(action, content);
			var result = await response.Content.ReadAsStringAsync();

			result = result;
			var i = 0;
			i++;
		}

	}
}

