using System;
using CoreMotion;
using Foundation;
using System.Collections.Generic;
using UIKit;

namespace CabFromHell.Xamarin.iOS
{
	public class Accellerometer : IAccellerometer
	{
		List<AccellerometerUpdate> _handlers = new List<AccellerometerUpdate>();


		public Accellerometer()
		{
			var accelleromeer = UIAccelerometer.SharedAccelerometer;

			accelleromeer.UpdateInterval = 0.5;
			accelleromeer.Acceleration += (sender, eventArgs) => {
				_handlers.ForEach(handler => handler(eventArgs.Acceleration.X, eventArgs.Acceleration.Y, eventArgs.Acceleration.Z));
			};


			/*
			var motionManager = new CMMotionManager ();
			motionManager.AccelerometerUpdateInterval = 0.5;

			motionManager.StartAccelerometerUpdates (NSOperationQueue.CurrentQueue,
				(data, error) => {
					_handlers.ForEach(handler => handler(data.Acceleration.X, data.Acceleration.Y, data.Acceleration.Z));
			 	 });*/
		}


		public void AddHandler (AccellerometerUpdate handler)
		{
			_handlers.Add (handler);
		}
	}
}

