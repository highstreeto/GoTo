using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace GoTo.Service.Domain {
    [DebuggerDisplay("Destination '{Name}'")]
    public class Destination : IEquatable<Destination> {
        public Destination(string name, double latitude, double longitude) {
            Name = name;
            Latitude = latitude;
            Longitude = longitude;
        }

        public string Name { get; }

        public double Latitude { get; }
        public double Longitude { get; }

        public override int GetHashCode() {
            return Name.GetHashCode();
        }

        public override bool Equals(object obj) {
            return Equals(obj as Destination);
        }

        public bool Equals(Destination other) {
            return other != null &&
                   Name == other.Name &&
                   Latitude == other.Latitude &&
                   Longitude == other.Longitude;
        }

        public override string ToString() {
            // TODO Proper display of long. and lat.
            return $"Destination '{Name}' at {Longitude} N {Latitude} E";
        }
    }
}