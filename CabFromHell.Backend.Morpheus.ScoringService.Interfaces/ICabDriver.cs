using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabFromHell.Backend.Morpheus.ScoringService.Interfaces
{
    public interface ICabDriver : IActor
    {
        Task SetNameAsync(string name);
        Task<string> GetNameAsync();
        Task<double> GetScoreAsync();
        Task<long> GetScoreUpdatesAsync();
        Task<DateTime> GetLastUpdatedAsync();
        Task UpdateScoreAsync(DateTime arrivalDate, double forceValue, int ammountOfMeasurements, double timeInterval);
    }
}
