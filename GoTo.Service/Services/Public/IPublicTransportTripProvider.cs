using GoTo.Service.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace GoTo.Service.Services {
    public interface IPublicTransportTripProvider {
        string Operator { get; }

        Task<IEnumerable<PublicTransportTrip>> SearchAsync(TripSearchRequest request);
    }
}
