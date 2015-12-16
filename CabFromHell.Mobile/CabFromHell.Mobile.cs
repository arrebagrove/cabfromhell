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


		public static void Register<TS,TI>()
		{
			var builder = new ContainerBuilder ();
			builder.RegisterGeneric (typeof(TI)).As (typeof(TS));
			builder.Update (Container);
		}


		public static void Register<TS>(TS implementation) where TS:class
		{
			var builder = new ContainerBuilder ();
			builder.RegisterInstance (implementation).As(typeof(TS));
			builder.Update (Container);
		}


		public App ()
		{
			MainPage = new CabFromHell.Mobile.MainPage ();
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

