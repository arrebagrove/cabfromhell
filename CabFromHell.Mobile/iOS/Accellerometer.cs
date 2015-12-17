using System;
using CoreMotion;
using Foundation;
using System.Collections.Generic;
using UIKit;

namespace CabFromHell.Mobile.iOS
{
	public class Accellerometer : IAccellerometer
	{
		List<AccellerometerUpdate> _handlers = new List<AccellerometerUpdate>();

		public Accellerometer()
		{
			var accelleromeer = UIAccelerometer.SharedAccelerometer;

			accelleromeer.UpdateInterval = 0.1;
			accelleromeer.Acceleration += (sender, eventArgs) => {
				_handlers.ForEach(handler => handler(eventArgs.Acceleration.X, eventArgs.Acceleration.Y, eventArgs.Acceleration.Z));
			};
		}

		public void AddHandler (AccellerometerUpdate handler)
		{
			_handlers.Add (handler);
		}
	}
}

