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
        /// <param name="name">Name of the searched destination (fuzzy match)</param>
        /// <param name="lat">Latitude of the searched destination (finds nearest)</param>
        /// <param name="lon">Longitude of the searched destination (finds nearest)</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Destination>), 200)]
        [ProducesResponseType(500)]
        public IActionResult Query(
                [FromQuery]string name,
                [FromQuery]double? lat,
                [FromQuery]double? lon
            ) {
            if (!string.IsNullOrWhiteSpace(name)) {
                var result = repo.FindByName(name)
                    .Select(domain => new Destination(domain));
                return result.Any()
                    ? Ok(result)
                    : (IActionResult)NotFound($"Location name '{name}' not matched!");
            } else if (lat.HasValue && lon.HasValue) {
                return repo.FindByGeo(lat.Value, lon.Value)
                    .WithException($"Location geo '{lat}, {lon}' not matched!")
                    .Map(domain => new Destination(domain))
                    .Match<IActionResult>(
                        some: dst => Ok(new[] { dst }),
                        none: msg => NotFound(msg)
                    );
            } else {
                if ((lat.HasValue && !lon.HasValue) || (!lat.HasValue && lon.HasValue))
                    return BadRequest("Both latitude and longitude must be specified!");
                return Ok(repo.Query().Select(o => new Destination(o)));
            }
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
