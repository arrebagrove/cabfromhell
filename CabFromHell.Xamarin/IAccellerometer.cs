using System;

namespace CabFromHell.Xamarin
{
	public delegate void AccellerometerUpdate(double x, double y, double z);

	public interface IAccellerometer
	{
		void AddHandler(AccellerometerUpdate handler);
	}
}

