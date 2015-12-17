using System;
using System.Collections.Generic;

using Xamarin.Forms;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using Autofac;
using System.Net;
using System.Globalization;

using System.Security.Cryptography;

namespace CabFromHell.Mobile
{
	public partial class MainPage : ContentPage
	{
		double _aggregatedLength;
		DateTime _lastSend;
		double _previousX;
		double _previousY;
		double _previousZ;
		int _count;
		Guid _tripId;
		bool _tripRunning;

		TimeSpan _tripCounter;


		public MainPage ()
		{
			InitializeComponent ();

			var accellerometer = App.Container.Resolve<IAccellerometer> ();

			_lastSend = DateTime.Now;

			startButton.IsVisible = true;
			stopButton.IsVisible = false;

			accellerometer.AddHandler (async (x, y, z) => {

				if( _tripRunning == false ) return;

				var deltaX = x - _previousX;
				var deltaY = y - _previousY;
				var deltaZ = z - _previousZ;

				_previousX = x;
				_previousY = y;
				_previousZ = z;

				var length = Math.Sqrt((deltaX*deltaX)+(deltaY*deltaY)+(deltaZ*deltaZ));
				_aggregatedLength += length;
				_count++;

				var now = DateTime.Now;
				var delta = now.Subtract(_lastSend);
				if( delta.TotalSeconds >= 5 ) {
					_lastSend = now;
					var aggregatedLength = _aggregatedLength;
					var count = _count;
					_aggregatedLength = 0;
					_count = 0;

					await SendEntry(driverEntry.Text, aggregatedLength, delta.TotalSeconds, count);


				}
			});
		}



		void StartClicked(object sender, EventArgs e) 
		{
			if (string.IsNullOrEmpty (driverEntry.Text)) return;

			_tripRunning = true;
			_tripId = Guid.NewGuid ();
			_tripCounter = new TimeSpan (0);

			startButton.IsVisible = false;
			stopButton.IsVisible = true;

			driverLabel.IsVisible = false;
			driverEntry.IsVisible = false;
			tripTime.IsVisible = true;
			_lastSend = DateTime.Now;

			Device.StartTimer (TimeSpan.FromSeconds (1), () => {
				Device.BeginInvokeOnMainThread(() => {
					tripTime.Text = $"{_tripCounter.Hours:D2}:{_tripCounter.Minutes:D2}:{_tripCounter.Seconds:D2}";
				});
				_tripCounter = _tripCounter.Add(TimeSpan.FromSeconds(1));

				if( _tripRunning == false ) return false;
				return true;
			});
		}

		void StopClicked(object sender, EventArgs e)
		{
			_tripRunning = false;
			tripTime.Text = "";
			startButton.IsVisible = true;
			stopButton.IsVisible = false;
			driverLabel.IsVisible = true;
			driverEntry.IsVisible = true;
			tripTime.IsVisible = false;
		}




		/*

		private static string createToken(string resourceUri, string key) //, string keyName)
		{
			TimeSpan sinceEpoch = DateTime.UtcNow - new DateTime(1970, 1, 1);
			var week = 60 * 60 * 24 * 7;
			var expiry = Convert.ToString((int)sinceEpoch.TotalSeconds + week);
			string stringToSign = WebUtility.UrlEncode(resourceUri) + "\n" + expiry;
			HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
			var signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));
			var sasToken = String.Format(CultureInfo.InvariantCulture, "SharedAccessSignature sr={0}&sig={1}&se={2}", WebUtility.UrlEncode(resourceUri), WebUtility.UrlEncode(signature), expiry);

			// &skn={3} keyName
			return sasToken;
		}
		*/


		// https://azure.microsoft.com/nb-no/documentation/articles/service-bus-sas-overview/

		async Task SendEntry(string source, double length, double totalSeconds, int count)
		{
			var buffer = $"{source}|{length:N4}|{totalSeconds:N4}|{count}";

			var httpClient = new HttpClient 
			{
				
				BaseAddress = new Uri("http://cabfromhellproxy.azurewebsites.net")
			};
			var action = $"api/cab?point={buffer}";

			var content = new StringContent(buffer, Encoding.UTF8, "application/json");
			var response = await httpClient.PostAsync(action, content);
			var result = await response.Content.ReadAsStringAsync();
			result = result;

			var i = 0;
			i++;

			#if(false)
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
			var device = "myFirstDevice";
			//var key = "9Avak8CKp9UeQqS7vCJ8IPqJuGgV0QEEL7i5iFg5mNg%3d"; //
			//var key = "gENd9WJ6otc2sEWXuS/CRwqxWjlqSY5TmodJOjF/Vgs=";
			//var key = "1dLTN4feSpJhzHQ1QtdxYKOTc5Z%2BchZJ8qmr0Za1m4Q";
			//var key = "jDw6ZKXaza9kFdEUmWH25tkrx43bSsStxBpt3UQ2Am0";
			//var key = "heZ9orRd7tcq7Mv5vXnlyDXMjc71Q2Xh7SPueBVSV/0=";

			var key = "Xrlz595lzPHsPxWdVmNGG6iC9IZwKTjMuBuA/UGQkrQ=";

			// HostName=hubtohellandback.azure-devices.net;DeviceId=myFirstDevice;SharedAccessKey=Xrlz595lzPHsPxWdVmNGG6iC9IZwKTjMuBuA/UGQkrQ=

			//var t = createToken ($"{serviceNamespace}.{host}/devices/{device}",key);

			//SharedAccessSignature sr=hubtohellandback.azure-devices.net&sig=jDw6ZKXaza9kFdEUmWH25tkrx43bSsStxBpt3UQ2Am0%3d&se=1450899958
			var sinceEpoch = DateTime.UtcNow - new DateTime(1970, 1, 1);
			var week = 60 * 60 * 24 * 7;
			var expiry = Convert.ToString((int)sinceEpoch.TotalSeconds + week);

			var encodedKey = WebUtility.UrlEncode (key);
			var encodedUrl = WebUtility.UrlEncode ($"{serviceNamespace}.{host}/devices/{device}");
			var sas = $"SharedAccessSignature sr={encodedUrl}&sig={encodedKey}&se={expiry}";

			// https%3A%2F%2F

			//var sas = "SharedAccessSignature sr=hubtohellandback.azure-devices.net&sig=jDw6ZKXaza9kFdEUmWH25tkrx43bSsStxBpt3UQ2Am0%3d&se=1450899958";	

			var action = $"devices/{device}/messages/events?api-version=2015-08-15-preview";

			var httpClient = new HttpClient
			{
				BaseAddress = new Uri($"https://{serviceNamespace}.{host}")
			};

			httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", sas);
			httpClient.DefaultRequestHeaders.TryAddWithoutValidation ("iothub-to", $"/devices/{device}");
			var content = new StringContent(buffer, Encoding.UTF8, "application/json");

			var contentType = "application/json";
				//"application/atom+xml;type=entry;charset=utf-8";
				//"application/json"; //;type=entry;charset=utf-8";
			content.Headers.Add("ContentType", contentType);
			var response = await httpClient.PostAsync(action, content);
			var result = await response.Content.ReadAsStringAsync();

			result = result;
			var i = 0;
			i++;
			#endif
		}


	}
}

