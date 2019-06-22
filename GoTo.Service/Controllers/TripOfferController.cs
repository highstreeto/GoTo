using Microsoft.AspNetCore.Mvc;
using System;
using Optional;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoTo.Service.Repositories;
using System.ComponentModel.DataAnnotations;
using Optional.Collections;

namespace GoTo.Service.Controllers {
    /// <summary>
    /// Controller for offered trips by users
    /// </summary>
    [Route("/api/tripoffer")]
    public class TripOfferController : ControllerBase {
        private readonly ITripOfferRepository repo;
        private readonly IDestinationRepository destRepo;

        public TripOfferController(ITripOfferRepository repo, IDestinationRepository destRepo) {
            this.repo = repo;
            this.destRepo = destRepo;
        }

        /// <summary>
        /// Query all available trips offered by users
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
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult Add([FromBody] TripOffer offer) {
            var domain = offer.ToDomain(destRepo);
            return domain.Match<IActionResult>(
                some: o => {
                    repo.Add(o);
                    return Ok(new TripOffer(o));
                },
                none: msg => BadRequest(msg)
            );
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

            public Option<Domain.TripOffer, string> ToDomain(IDestinationRepository destRepo) {
                return
                    destRepo.FindByName(StartLocation)
                    .FirstOrNone()
                    .WithException($"Start location '{StartLocation}' could not matched!")
                    .FlatMap(start =>
                        destRepo.FindByName(EndLocation)
                        .FirstOrNone()
                        .WithException($"End location '{EndLocation}' could not matched!")
                        .Map(end => new Domain.TripOffer(StarTime, start,
                            EndTime - StarTime, end,
                            new Domain.User(null, OfferedBy))
                        )
                );
            }
        }
    }
}
