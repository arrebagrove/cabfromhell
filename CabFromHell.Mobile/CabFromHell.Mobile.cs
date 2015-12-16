using System;

using Xamarin.Forms;

namespace CabFromHell.Mobile
{
	public class App : Application
	{
		public App (IAccellerometer accellerometer)
		{
			MainPage = new CabFromHell.Mobile.MainPage ();


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
	}
}

