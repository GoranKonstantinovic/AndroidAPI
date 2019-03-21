using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Owin;
using PMSIU_API.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMSIU_API
{
    public class ApplicationUserManager : UserManager<UserModel>
    {
        public ApplicationUserManager(IUserStore<UserModel> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<UserModel>(context.Get<AuthContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<UserModel>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<UserModel>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }
}