using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoTo.Service.Domain;

namespace GoTo.Service.Repositories {
    public class InMemoryDestinationRepository : IDestinationRepository {
        private readonly List<Destination> destinations;

        public InMemoryDestinationRepository() {
            destinations = new List<Destination>();
        }

        public IEnumerable<Destination> Query()
            => destinations;

        public Destination FindByName(string name) {
            // TODO Implement fuzzy search
            return Query()
                .Where(dst => dst.Name == name)
                .SingleOrDefault();
        }

        public Destination FindByGeo(double lat, double lon) {
            // TODO Implement fuzzy search
            return Query()
                .Where(dst => dst.Latitude == lat && dst.Longitude == lon)
                .SingleOrDefault();
        }

        public void Add(Destination destination) {
            destinations.Add(destination);
        }
    }
}
