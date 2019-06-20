using System;
using System.Collections.Generic;
using System.Text;

namespace GoTo.Lambda.Domain {
    public class Trip {
        public TripKind Kind { get; set; }

        public DateTime StartTime { get; set; }
        public string StartLocation { get; set; }
        public DateTime EndTime { get; set; }
        public string EndLocation { get; set; }
        public TimeSpan Duration => EndTime - StartTime;
        /// <summary>
        /// Name of the user (if OfferedByUser) or company (if PublicTransport)
        /// </summary>
        public string Provider { get; set; }
    }

    public enum TripKind {
        PublicTransport,
        OfferedByUser
    }
}
