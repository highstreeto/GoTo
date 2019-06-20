using GoTo.Service.Domain;
using Microsoft.Extensions.Options;
using Optional;
using Optional.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GoTo.Service.Repositories {
    public class InMemoryDestinationRepository : IDestinationRepository {
        private readonly List<Destination> destinations;

        public InMemoryDestinationRepository(IOptionsMonitor<Settings> options) {
            destinations = options.CurrentValue.Destinations
                .Select(m => new Destination(m))
                .ToList();
        }

        public IEnumerable<Destination> Query()
            => destinations;

        public Option<Destination> FindByName(string name) {
            var strComp = StringComparison.CurrentCultureIgnoreCase;
            return Query()
                .Where(dst
                    => string.Equals(dst.Name, name, strComp)
                    || dst.Name.Contains(name, strComp)
                )
                .SingleOrNone();
        }

        public Option<Destination> FindByGeo(double lat, double lon) {
            return Query()
                .Select(d => (dest: d, dist: d.DistanceTo(lat, lon)))
                .OrderBy(d => d.dist)
                .Select(d => d.dest)
                .FirstOrNone();
        }

        public void Add(Destination destination) {
            destinations.Add(destination);
        }

        public class Settings {
            public Destination.Memento[] Destinations { get; set; }
        }
    }
}
