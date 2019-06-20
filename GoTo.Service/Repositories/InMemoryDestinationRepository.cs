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

        public InMemoryDestinationRepository(IOptions<Settings> options) {
            destinations = options.Value.Destinations
                .Select(m => new Destination(m))
                .ToList();
        }

        public IEnumerable<Destination> Query()
            => destinations;

        public Option<Destination> FindByName(string name) {
            var strComp = StringComparison.CurrentCultureIgnoreCase;
            var result = Query()
                .Where(dst
                    => string.Equals(dst.Name, name, strComp)
                    || dst.Name.Contains(name, strComp)
                );

            if (result.Any())
                return result.SingleOrNone();
            else
                return Query()
                    .Select(d => (dest: d, levdist: Utils.LevenshteinDistance(d.Name, name)))
                    .Where(d => d.levdist < 3)
                    .OrderBy(d => d.levdist)
                    .Select(d => d.dest)
                    .FirstOrNone();
        }

        public Option<Destination> FindByGeo(double lat, double lon) {
            return Query()
                .Select(d => (dest: d, dist: d.DistanceTo(lat, lon)))
                .Where(d => d.dist < 100)
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
