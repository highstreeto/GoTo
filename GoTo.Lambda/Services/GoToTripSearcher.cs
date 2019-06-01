using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using GoTo.Lambda.Domain;
using Newtonsoft.Json;

namespace GoTo.Lambda.Services {
    public class GoToTripSearcher : ITripSearcher {
        private readonly string host;

        public GoToTripSearcher(string host) {
            this.host = host;
        }

        public async Task<IEnumerable<Trip>> SearchForTripsAsync(string start, string end, DateTime time) {
            var client = new HttpClient();
            var searchParams = new TripSearchParams() {
                StartLocation = start,
                EndLocation = end,
                StartTime = time
            };

            var response = await client.PostAsync($"{host}/api/trip/search",
                new StringContent(JsonConvert.ToString(searchParams), Encoding.UTF8, MediaTypeNames.Application.Json));
            if (response.IsSuccessStatusCode) {
                var trips = JsonConvert.DeserializeObject<Trip[]>(
                    await response.Content.ReadAsStringAsync()
                );
                return trips;
            } else {
                // TODO Better error handling
                return new Trip[] { };
            }
        }
    }

    public class TripSearchParams {
        public string StartLocation { get; set; }
        public DateTime StartTime { get; set; }
        public string EndLocation { get; set; }
    }
}
