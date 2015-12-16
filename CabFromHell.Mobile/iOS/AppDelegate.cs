using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace CabFromHell.Mobile.iOS
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			global::Xamarin.Forms.Forms.Init ();

			App.Register<IAccellerometer> (new Accellerometer ());

			var accellerometer = new Accellerometer ();
			LoadApplication (new App ());

			return base.FinishedLaunching (app, options);
		}
	}
}

