using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PMSIU_API.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace PMSIU_API.Utilities
{
    public class AuthRepository : IDisposable
    {
        private AuthContext authContext;
        private UserManager<UserModel> userManager;

        public AuthRepository()
        {
            authContext = new AuthContext();
            userManager = new UserManager<UserModel>(new UserStore<UserModel>(authContext));
        }

        public async Task<IdentityResult> RegisterUser(UserModel userModel)
        {
            userModel.Email = userModel.UserName;
            var result = await userManager.CreateAsync(userModel, userModel.Password);
            return result;
        }

        public async Task<UserModel> FindUser(string userName, string password)
        {
            return (await userManager.FindAsync(userName, password)) as UserModel;
        }

        public void Dispose()
        {
            authContext.Dispose();
            userManager.Dispose();
        }
    }
}