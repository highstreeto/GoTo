using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoTo.Service.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GoTo.Service.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository repo;

        public UserController(IUserRepository repo) {
            this.repo = repo;
        }

        /// <summary>
        /// Query all registered users
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<User>), 200)]
        [ProducesResponseType(500)]
        public IActionResult Query() {
            return Ok(repo.Query().Select(o => new User(o)));
        }

        public class User {
            public string LoginName { get; set; }
            public string DisplayName { get; set; }

            public User() { }

            public User(Domain.User domain) {
                LoginName = domain.LoginName;
                DisplayName = domain.DisplayName;
            }
        }
    }
}
