using Microsoft.AspNet.Identity;
using PMSIU_API.Models.Identity;
using PMSIU_API.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace PMSIU_API.Controllers
{
    public class AccountController : ApiController
    {
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(UserModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using (var repository = new AuthRepository())
            {
                IdentityResult result = await repository.RegisterUser(model);

                if (!result.Succeeded)
                {
                    return BadRequest();
                }

                return Ok();
            }
        }
    }
}
