using GoTo.Lambda.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoTo.Lambda.Services {
    public interface ITripSearcher {
        Task<IEnumerable<Trip>> SearchForTripsAsync(string start, string end, DateTime time);
    }

    public class TripSearcherFake : ITripSearcher {
        public Task<IEnumerable<Trip>> SearchForTripsAsync(string start, string end, DateTime time) {
            return Task.FromResult(new List<Trip>() {
                new Trip() {
                    StartLocation = "Hagenberg im Mühlkreis",
                    StartTime = time.AddHours(0.5),
                    EndLocation = "Linz/Donau",
                    EndTime = time.AddHours(1.5),
                    Kind = TripKind.PublicTransport
                },
                new Trip() {
                    StartLocation = "Hagenberg im Mühlkreis",
                    StartTime = time.AddHours(0.75),
                    EndLocation = "Linz/Donau",
                    EndTime = time.AddHours(1.25),
                    Kind = TripKind.OfferedByUser
                },
                new Trip() {
                    StartLocation = "Hagenberg im Mühlkreis",
                    StartTime = time.AddHours(1),
                    EndLocation = "Linz/Donau",
                    EndTime = time.AddHours(1.45),
                    Kind = TripKind.OfferedByUser
                }
            }.AsEnumerable());
        }
    }
}
