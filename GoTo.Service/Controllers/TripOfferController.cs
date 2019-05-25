using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoTo.Service.Repositories;
using System.ComponentModel.DataAnnotations;

namespace GoTo.Service.Controllers {
    /// <summary>
    /// Controller for offered trips by users
    /// </summary>
    [Route("/api/tripoffer")]
    public class TripOfferController : ControllerBase {
        private readonly ITripOfferRepository repo;

        public TripOfferController(ITripOfferRepository repo) {
            this.repo = repo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TripOffer>), 200)]
        [ProducesResponseType(500)]
        public IActionResult Query() {
            return Ok(repo.Query().Select(o => new TripOffer(o)));
        }

        /// <summary>
        /// Adds a new offer for a trip
        /// </summary>
        /// <param name="offer">Offered trip</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(TripOffer), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public IActionResult Add([FromBody] TripOffer offer) {
            var domain = offer.ToDomain();
            repo.Add(domain);
            return Ok(new TripOffer(domain));
        }

        public class TripOffer {
            public TripOffer() { }

            public TripOffer(Domain.TripOffer offer) {
                StartLocation = offer.StartLocation.Name;
                StarTime = offer.StartTime;
                EndTime = offer.StartTime + offer.EstimatedDuration;
                EndLocation = offer.EndLocation.Name;
                OfferedBy = offer.OfferedBy.DisplayName;
            }

            [Required]
            public string StartLocation { get; set; }
            [Required]
            public DateTime StarTime { get; set; }
            [Required]
            public DateTime EndTime { get; set; }
            [Required]
            public string EndLocation { get; set; }
            [Required]
            public string OfferedBy { get; set; }

            public Domain.TripOffer ToDomain() {
                return new Domain.TripOffer(
                    StarTime, new Domain.Destination(StartLocation, 0, 0),
                    EndTime - StarTime, new Domain.Destination(EndLocation, 0, 0),
                    new Domain.User(null, OfferedBy)
                );
            }
        }
    }
}
