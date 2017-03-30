using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Mpi.Auth.Api.Controllers
{
    [Route("identity")]
    [Authorize]
    public class IdentityController : ControllerBase {
        // GET api/values
        [HttpGet]
        public IActionResult Get() {
            return new JsonResult(User.Claims.Select(claim => new { claim.Type, claim.Value }));
        }

    }
}
