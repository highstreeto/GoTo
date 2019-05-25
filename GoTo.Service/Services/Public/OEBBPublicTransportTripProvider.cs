using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoTo.Service.Domain;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace GoTo.Service.Services {
    public class OEBBPublicTransportTripProvider : IPublicTransportTripProvider {
        public string Operator => "ÖBB";

        public Task<IEnumerable<PublicTransportTrip>> SearchAsync(TripSearchRequest request) {
            throw new NotImplementedException();
        }
    }
}
