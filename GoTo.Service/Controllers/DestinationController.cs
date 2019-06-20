using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoTo.Service.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GoTo.Service.Controllers
{
    [Route("api/destination")]
    [ApiController]
    public class DestinationController : ControllerBase
    {
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
