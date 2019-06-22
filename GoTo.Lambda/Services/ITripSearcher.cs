using GoTo.Lambda.Domain;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GoTo.Lambda.Services {
    public interface ITripSearcher {
        Task<IEnumerable<Destination>> FindDestinationByName(string name);

        Task<IEnumerable<Destination>> FindDestinationByGeo(double lat, double lon);

        Task<IEnumerable<Trip>> SearchForTripsAsync(string start, string end, Instant time);
    }
}
