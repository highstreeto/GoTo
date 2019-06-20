using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GoTo.Service.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Optional;

namespace GoTo.Service.Controllers {
    [Route("api/destination")]
    [ApiController]
    public class DestinationController : ControllerBase {
        private readonly IDestinationRepository repo;

        public DestinationController(IDestinationRepository destRepo) {
            this.repo = destRepo;
        }

        /// <summary>
        /// Query all available destinations
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Destination>), 200)]
        [ProducesResponseType(500)]
        public IActionResult Query() {
            return Ok(repo.Query().Select(o => new Destination(o)));
        }

        /// <summary>
        /// Query all available destinations
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Destination>), 200)]
        [ProducesResponseType(500)]
        public IActionResult Find([FromBody] DestinationSearchParams search) {
            var result = Option.None<Domain.Destination, string>("");
            switch (search.Mode) {
                case SearchMode.Name:
                    result = repo.FindByName(search.Name)
                        .WithException($"Location name '{search.Name}' not matched!");
                    break;
                case SearchMode.Geo:
                    result = repo.FindByGeo(search.Latitude, search.Longitude)
                        .WithException($"Location geo {search.Latitude}, {search.Longitude} not matched!");
                    break;
                default:
                    return BadRequest("Unknown search mode!");
            }

            return result
                .Map(domain => new Destination(domain))
                .Match<IActionResult>(
                    some: dst => Ok(dst),
                    none: msg => NotFound(msg)
                );
        }

        public class DestinationSearchParams {
            [Required]
            public SearchMode Mode { get; set; }
            public string Name { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }

        public enum SearchMode {
            Name,
            Geo
        }

        public class Destination {
            public string Name { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }

            public Destination() { }

            public Destination(Domain.Destination domain) {
                Name = domain.Name;
                Latitude = domain.Latitude;
                Longitude = domain.Longitude;
            }
        }
    }
}
