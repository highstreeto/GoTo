using GoTo.Service.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GoTo.Service.Controllers {
    /// <summary>
    /// 
    /// </summary>
    [Route("/api/trip")]
    public class TripController : ControllerBase {
        private readonly ITripSearcher searcher;

        public TripController(ITripSearcher searcher) {
            this.searcher = searcher;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchParams"></param>
        [HttpPost]
        [Route("search")]
        [ProducesResponseType(typeof(IEnumerable<FoundTrip>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SearchAsync([FromBody] TripSearchParams searchParams) {
            var results = await searcher.SearchAsync(searchParams.ToService());
            return Ok(results.Select(t => new FoundTrip(t)));
        }

        /// <summary>
        /// 
        /// </summary>
        public class TripSearchParams {
            /// <summary>
            /// 
            /// </summary>
            [Required]
            public string StartLocation { get; set; }
            /// <summary>
            /// 
            /// </summary>
            [Required]
            public DateTime StartTime { get; set; }
            /// <summary>
            /// 
            /// </summary>
            [Required]
            public string EndLocation { get; set; }

            public Services.TripSearchRequest ToService() {
                return new TripSearchRequest(
                    new Domain.Destination(StartLocation, 0, 0),
                    new Domain.Destination(EndLocation, 0, 0),
                    StartTime
                );
            }
        }

        public class FoundTrip {
            public FoundTrip() { }

            public FoundTrip(Domain.Trip trip) {
                if (trip is Domain.PublicTransportTrip) {
                    Kind = FoundTripKind.PublicTransport;
                }
                if (trip is Domain.TripOffer) {
                    Kind = FoundTripKind.OfferedByUser;
                }

                StartTime = trip.StartTime;
                StartLocation = trip.StartLocation.Name;
                EndTime = trip.StartTime + trip.EstimatedDuration;
                EndLocation = trip.EndLocation.Name;
            }

            public FoundTripKind Kind { get; set; }

            public DateTime StartTime { get; set; }
            public string StartLocation { get; set; }
            public DateTime EndTime { get; set; }
            public string EndLocation { get; set; }
        }

        public enum FoundTripKind {
            PublicTransport,
            OfferedByUser
        }
    }
}
