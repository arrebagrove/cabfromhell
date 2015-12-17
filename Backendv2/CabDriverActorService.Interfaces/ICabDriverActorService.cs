using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace CabDriverActorService.Interfaces
{
    /// <summary>
    /// This interface represents the actions a client app can perform on an actor.
    /// It MUST derive from IActor and all methods MUST return a Task.
    /// </summary>
    public interface ICabDriverActorService : IActor
    {
        Task SetNameAsync(string name);
        Task<string> GetNameAsync();
        Task<double> GetScoreAsync();
        Task<long> GetScoreUpdatesAsync();
        Task<DateTime> GetLastUpdatedAsync();
        Task UpdateScoreAsync(DateTime arrivalDate, double forceValue, int ammountOfMeasurements, double timeInterval);
    }
}
