using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace GoTo.Service.Domain {
    [DebuggerDisplay("Destination '{Name}'")]
    public class Destination {
        public Destination(Memento memento)
            : this(memento.Name, memento.Lat, memento.Lon) { }

        public Destination(string name, double latitude, double longitude) {
            Name = name;
            Latitude = latitude;
            Longitude = longitude;
        }

        public string Name { get; }

        public double Latitude { get; }
        public double Longitude { get; }

        public double DistanceTo(Destination other)
            => DistanceTo(other.Latitude, other.Longitude);

        public double DistanceTo(double lat, double lon) {
            var src = new Geo.Coordinate(Latitude, Longitude);
            var dest = new Geo.Coordinate(lat, lon);
            var path = new Geo.Geometries.LineString(src, dest);
            return path.GetLength()
                .ConvertTo(Geo.Measure.DistanceUnit.Km)
                .Value;
        }

        public override string ToString() {
            // TODO Proper display of long. and lat.
            return $"Destination '{Name}' at {Longitude} N {Latitude} E";
        }

        public class Memento {
            public string Name { get; set; }
            public double Lat { get; set; }
            public double Lon { get; set; }
        }
    }
}
