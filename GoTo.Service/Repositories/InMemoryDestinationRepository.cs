using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoTo.Service.Domain;

namespace GoTo.Service.Repositories {
    public class InMemoryDestinationRepository : IDestinationRepository {
        private readonly List<Destination> destinations;

        public InMemoryDestinationRepository() {
            destinations = new List<Destination>() {
                new Destination("Waidhofen an der Ybbs", 47.960310, 14.772283),
                new Destination("Linz/Donau", 48.305598, 14.286601),
                new Destination("Hagenberg im Mühlkreis", 48.367126, 14.516660),
                new Destination("Wien", 48.208344, 16.371313),
            };
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
            return Query()
                .Select(d => (dest: d, dist: d.DistanceTo(lat, lon)))
                .OrderBy(d => d.dist)
                .Select(d => d.dest)
                .FirstOrDefault();
        }

        public void Add(Destination destination) {
            destinations.Add(destination);
        }
    }
}
