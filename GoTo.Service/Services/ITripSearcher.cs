using GoTo.Service.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace GoTo.Service.Services {
    public interface ITripSearcher {
        Task<IEnumerable<Trip>> SearchAsync(TripSearchRequest request);
    }

    public class TripSearchRequest {
        public TripSearchRequest(
            Destination start,
            Destination end,
            DateTime startTime) {
            Start = start;
            End = end;
            StartTime = startTime;
        }

        public Destination Start { get; }
        public Destination End { get; }

        public DateTime StartTime { get; }
    }
}
