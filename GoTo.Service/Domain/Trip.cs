using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace GoTo.Service.Domain {
    public abstract class Trip {
        protected Trip(DateTime startTime, Destination startLocation, TimeSpan estimatedDuration, Destination endLocation) {
            StartTime = startTime;
            StartLocation = startLocation;
            EstimatedDuration = estimatedDuration;
            EndLocation = endLocation;
        }

        public DateTime StartTime { get; }
        public Destination StartLocation { get; set; }

        public TimeSpan EstimatedDuration { get; }
        public Destination EndLocation { get; set; }
    }

    public class TripOffer : Trip {
        public TripOffer(
                DateTime startTime, Destination startLocation,
                TimeSpan estimatedDuration, Destination endLocation,
                User offeredBy) : base(startTime, startLocation, estimatedDuration, endLocation) {
            OfferedBy = offeredBy;
        }

        public User OfferedBy { get; }
    }

    public class PublicTransportTrip : Trip {
        public PublicTransportTrip(
                DateTime startTime, Destination startLocation,
                TimeSpan estimatedDuration, Destination endLocation,
                string @operator, PublicTransportType type) : base(startTime, startLocation, estimatedDuration, endLocation) {
            Operator = @operator;
            Type = type;
        }

        public string Operator { get; }
        public PublicTransportType Type { get; }
    }

    [Flags]
    public enum PublicTransportType {
        Unspecified = 0,
        Bus = 1 << 0,
        Train = 1 << 2,
        Plane = 1 << 3,
        Boat = 1 << 4
    }
}
