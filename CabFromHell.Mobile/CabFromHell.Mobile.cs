using System;
using System.Net.Http;

using Xamarin.Forms;
using System.Threading.Tasks;
using System.Text;

namespace CabFromHell.Mobile
{
	public class App : Application
	{
		private const string DeviceConnectionString = "HostName=hubtohellandback.azure-devices.net;DeviceId=CrazyCabA;SharedAccessKey=heZ9orRd7tcq7Mv5vXnlyDXMjc71Q2Xh7SPueBVSV/0=";

		public App (IAccellerometer accellerometer)
		{
			MainPage = new CabFromHell.Mobile.MainPage ();

			var sharedAccessKey = "heZ9orRd7tcq7Mv5vXnlyDXMjc71Q2Xh7SPueBVSV/0=";
			var deviceId = "CrazyCabA";


			SendEntry ("{x:1}");

			accellerometer.AddHandler (async (x, y, z) => {
				var json = $"{{ x: {x}, y: {y}, z: {z} }} ";
				await SendEntry(json);
			});

			//var client = new EventHubSasClient (



			/*
			var label = new Label { XAlign = TextAlignment.Center, Text = "Yo" };

			// The root page of your application
			MainPage = new ContentPage {
				Content = new StackLayout {
					VerticalOptions = LayoutOptions.Center,
					Children = {
						label
					}
				}
			};

			accellerometer.AddHandler ((x, y, z) => label.Text = $"{x}, {y}, {z}");*/
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}



		async Task SendEntry(string json)
		{
			//var json = _serializer.ToJson(entry);

			// Generate the shared access SAS URL with the tool from the link above


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
			var hubName = "activity";
			var device = "CrazyCabA";
			var key = "1dLTN4feSpJhzHQ1QtdxYKOTc5Z%2BchZJ8qmr0Za1m4Q%3D";
			var ttl = "1450280372";

			var sas = $"SharedAccessSignature sr=https%3a%2f%2f{serviceNamespace}.{host}%2devices%{device}&sig={key}&se={ttl}";

			var action = $"devices/{device}/messages/events?api-version=2015-08-15-preview";

			//var url = string.Format("{0}/publishers/{partition}/{eventhub}", hubName);


			var httpClient = new HttpClient
			{
				BaseAddress = new Uri($"https://{serviceNamespace}.{host}")
			};

			httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", sas);
			var content = new StringContent(json, Encoding.UTF8, "application/json");

			var contentType = "application/json;type=entry;charset=utf-8";
			content.Headers.Add("ContentType", contentType);
			var response = await httpClient.PostAsync(action, content);
			var result = await response.Content.ReadAsStringAsync();

			var i = 0;
			i++;
		}

	}
}

