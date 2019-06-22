using GoTo.Service.Repositories;
using GoTo.Service.Services;
using Microsoft.AspNetCore.Mvc;
using Optional.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GoTo.Service.Controllers {
    /// <summary>
    /// 
    /// </summary>
    [Route("/api/tripsearch")]
    public class TripSearchController : ControllerBase {
        private readonly ITripSearcher searcher;
        private readonly IDestinationRepository destRepo;

        public TripSearchController(ITripSearcher searcher,
            IDestinationRepository destRepo) {
            this.searcher = searcher;
            this.destRepo = destRepo;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchParams"></param>
        [HttpPost]
        [ProducesResponseType(typeof(IEnumerable<FoundTrip>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Search([FromBody] TripSearchParams searchParams) {
            var request = searchParams.ToService(destRepo);
            if (request.Start == null)
                return BadRequest($"Start location '{searchParams.StartLocation}' could not be matched!");
            if (request.End == null)
                return BadRequest($"End location '{searchParams.StartLocation}' could not be matched!");

            var results = await searcher.SearchAsync(request);
            return Ok(results.Select(t => new FoundTrip(t)));
        }

        /// <summary>
        /// Parameters for a trip search
        /// </summary>
        public class TripSearchParams {
            /// <summary>
            /// Name of the start location
            /// </summary>
            [Required]
            public string StartLocation { get; set; }
            /// <summary>
            /// Earliest trip start time
            /// </summary>
            [Required]
            public DateTime StartTime { get; set; }
            /// <summary>
            /// Name of the end location
            /// </summary>
            [Required]
            public string EndLocation { get; set; }

            public Services.TripSearchRequest ToService(IDestinationRepository repo) {
                return new TripSearchRequest(
                    // TODO Refactor
                    repo.FindByName(StartLocation).FirstOrNone().ValueOr((Domain.Destination)null),
                    repo.FindByName(EndLocation).FirstOrNone().ValueOr((Domain.Destination)null),
                    StartTime
                );
            }
        }

        public class FoundTrip {
            public FoundTrip() { }

            public FoundTrip(Domain.Trip trip) {
                if (trip is Domain.PublicTransportTrip ptt) {
                    Kind = FoundTripKind.PublicTransport;
                    Provider = ptt.Operator;
                }
                if (trip is Domain.TripOffer to) {
                    Kind = FoundTripKind.OfferedByUser;
                    Provider = to.OfferedBy.DisplayName;
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
            public string Provider { get; set; }
        }

        public enum FoundTripKind {
            PublicTransport,
            OfferedByUser
        }
    }
}
