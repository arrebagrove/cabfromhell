using System;
using System.Net.Http;

using Xamarin.Forms;
using System.Threading.Tasks;
using System.Text;
using Autofac;

namespace CabFromHell.Mobile
{
	public class App : Application
	{
		public static readonly IContainer Container;

		static App()
		{
			var builder = new ContainerBuilder ();
			Container = builder.Build ();
		}

		private const string DeviceConnectionString = "HostName=hubtohellandback.azure-devices.net;DeviceId=CrazyCabA;SharedAccessKey=heZ9orRd7tcq7Mv5vXnlyDXMjc71Q2Xh7SPueBVSV/0=";

		public App ()
		{
			MainPage = new CabFromHell.Mobile.MainPage ();

			/*
			accellerometer.AddHandler (async (x, y, z) => {
				await SendEntry("HorseManeuerMan", x,y,z);
			});*/
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



		async Task SendEntry(string source, double x, double y, double z)
		{

			var buffer = $"{source}|{x:N2}|{y:N2}|{z:N2}";


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
			var device = "CrazyCabA";
			var key = "1dLTN4feSpJhzHQ1QtdxYKOTc5Z%2BchZJ8qmr0Za1m4Q%3D";
			var ttl = "1450280372";

			var sas = $"SharedAccessSignature sr={serviceNamespace}.{host}%2Fdevices%2F{device}&sig={key}&se={ttl}";

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

