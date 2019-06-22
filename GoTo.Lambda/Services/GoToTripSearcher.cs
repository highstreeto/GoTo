using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using GoTo.Lambda.Domain;
using Newtonsoft.Json;
using NodaTime;

namespace GoTo.Lambda.Services {
    public class GoToTripSearcher : ITripSearcher {
        private readonly string host;

        public GoToTripSearcher(string host) {
            this.host = host;
        }

        public async Task<IEnumerable<Destination>> FindDestinationByName(string name) {
            var client = new HttpClient();
            var response = await client.GetAsync(
                $"{host}/api/destination" +
                $"?name={Uri.EscapeUriString(name)}");
            if (response.IsSuccessStatusCode) {
                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response success! Got: {json}");

                var result = JsonConvert.DeserializeObject<Destination[]>(json);
                return result;
            } else {
                Console.WriteLine($"Response for {nameof(FindDestinationByName)} was unsuccessful! Code: {response.StatusCode}, Error: {await response.Content.ReadAsStringAsync()}");
                // TODO Better error handling
                return new Destination[] { };
            }
        }

        public async Task<IEnumerable<Destination>> FindDestinationByGeo(double lat, double lon) {
            var client = new HttpClient();
            var response = await client.GetAsync(
                $"{host}/api/destination" +
                $"?lat={lat.ToString(CultureInfo.InvariantCulture)}" +
                $"&lon={lon.ToString(CultureInfo.InvariantCulture)}");
            if (response.IsSuccessStatusCode) {
                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response success! Got: {json}");

                var result = JsonConvert.DeserializeObject<Destination[]>(json);
                return result;
            } else {
                Console.WriteLine($"Response for {nameof(FindDestinationByGeo)} was unsuccessful! Code: {response.StatusCode}, Error: {await response.Content.ReadAsStringAsync()}");
                // TODO Better error handling
                return new Destination[] { };
            }
        }

        public async Task<IEnumerable<Trip>> SearchForTripsAsync(string start, string end, DateTime time) {
            var client = new HttpClient();
            var searchParams = new TripSearchParams() {
                StartLocation = start,
                EndLocation = end,
                StartTime = time
            };

            var request = JsonConvert.SerializeObject(searchParams);
            Console.WriteLine($"Request: {request}");

            var response = await client.PostAsync($"{host}/api/tripsearch",
                new StringContent(request, Encoding.UTF8, MediaTypeNames.Application.Json));
            if (response.IsSuccessStatusCode) {
                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response success! Got: {json}");

                var trips = JsonConvert.DeserializeObject<Trip[]>(json);
                return trips;
            } else {
                Console.WriteLine($"Response for {nameof(SearchForTripsAsync)} was unsuccessful! Code: {response.StatusCode}, Error: {await response.Content.ReadAsStringAsync()}");
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
