using GoTo.Lambda.Domain;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTo.Lambda.Services {
    public class TripSearcherFake : ITripSearcher {
        private List<Destination> destinations;

        public TripSearcherFake() {
            this.destinations = new List<Destination>() {
                new Destination() {
                    Name =  "Waidhofen an der Ybbs",
                    Latitude = 47.960310,
                    Longitude = 14.772283
                },
                new Destination() {
                    Name =  "Linz",
                    Latitude = 48.305598,
                    Longitude = 14.286601
                },
                new Destination() {
                    Name =  "Hagenberg im Mühlkreis, Ortsmitte",
                    Latitude = 48.367126,
                    Longitude = 14.516660
                },
                new Destination() {
                    Name =  "Wien",
                    Latitude = 48.208344,
                    Longitude = 16.371313
                }
            };
        }

        public Task<IEnumerable<Destination>> FindDestinationByName(string name) {
            var strComp = StringComparison.CurrentCultureIgnoreCase;
            var result = destinations
                .Where(dst
                    => string.Equals(dst.Name, name, strComp)
                    || dst.Name.Contains(name, strComp)
                );

            if (result.Any()) // found exact or containing match -> return first
                return Task.FromResult(new[] { result.First() }.AsEnumerable());

            // continue with fuzzy matching via Levenshtein dist.
            var fuzzy = destinations
                .Select(d => (dest: d, levdist: Utils.LevenshteinDistance(d.Name.ToLower(), name.ToLower())))
                .OrderBy(d => d.levdist);
            if (fuzzy.First().levdist <= 2) // found good enough match
                return Task.FromResult(new[] { fuzzy.First().dest }.AsEnumerable());

            return Task.FromResult(fuzzy
                .Select(d => d.dest));
        }

        public Task<IEnumerable<Destination>> FindDestinationByGeo(double lat, double lon) {
            return Task.FromResult<IEnumerable<Destination>>(
                new[] { destinations.First() }
            );
        }

        public Task<IEnumerable<Trip>> SearchForTripsAsync(string start, string end, Instant time) {
            return Task.FromResult(new List<Trip>() {
                new Trip() {
                    StartLocation = "Hagenberg im Mühlkreis",
                    StartTime = DateTime.Now,
                    EndLocation = "Linz/Donau",
                    EndTime = DateTime.Now.AddHours(1.5),
                    Kind = TripKind.PublicTransport,
                    Provider = "ÖBB"
                },
                new Trip() {
                    StartLocation = "Hagenberg im Mühlkreis",
                    StartTime = DateTime.Now.AddHours(0.25),
                    EndLocation = "Linz/Donau",
                    EndTime = DateTime.Now.AddHours(1.25),
                    Kind = TripKind.OfferedByUser,
                    Provider = "Erika"
                },
                new Trip() {
                    StartLocation = "Hagenberg im Mühlkreis",
                    StartTime = DateTime.Now.AddHours(1),
                    EndLocation = "Linz/Donau",
                    EndTime = DateTime.Now.AddHours(1.45),
                    Kind = TripKind.OfferedByUser,
                    Provider = "Max"
                }
            }.AsEnumerable());
        }
    }

    public static class Utils {
        // From: https://rosettacode.org/wiki/Levenshtein_distance#C.23
        public static int LevenshteinDistance(string s, string t) {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            if (n == 0) {
                return m;
            }

            if (m == 0) {
                return n;
            }

            for (int i = 0; i <= n; i++)
                d[i, 0] = i;
            for (int j = 0; j <= m; j++)
                d[0, j] = j;

            for (int j = 1; j <= m; j++)
                for (int i = 1; i <= n; i++)
                    if (s[i - 1] == t[j - 1])
                        d[i, j] = d[i - 1, j - 1];  //no operation
                    else
                        d[i, j] = Math.Min(Math.Min(
                            d[i - 1, j] + 1,    //a deletion
                            d[i, j - 1] + 1),   //an insertion
                            d[i - 1, j - 1] + 1 //a substitution
                            );
            return d[n, m];
        }
    }
}
