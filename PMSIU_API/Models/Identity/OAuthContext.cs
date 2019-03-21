using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMSIU_API.Models.Identity
{
    public class AuthContext : IdentityDbContext<UserModel>
    {
        public AuthContext()
         : base("OAuthContext", throwIfV1Schema: false)
        {
        }

        public static AuthContext Create()
        {
            return new AuthContext();
        }
    }
}