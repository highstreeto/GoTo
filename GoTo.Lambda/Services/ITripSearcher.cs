using GoTo.Lambda.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GoTo.Lambda.Services {
    public interface ITripSearcher {
        Task<IEnumerable<Destination>> FindDestinationByName(string name);

        Task<Destination> FindDestinationByGeo(double lat, double lon);

        Task<IEnumerable<Trip>> SearchForTripsAsync(string start, string end, DateTime time);
    }
}
