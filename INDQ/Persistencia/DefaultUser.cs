using Dominio;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistencia
{
   public class DefaultUser
    {
        public static async Task InsertData(IndqContext context,UserManager<User> userManager)
        {
            if (!userManager.Users.Any())
            {
                var objectUser = new User
                {
                    FirstName = "FirstNameTEst",
                    LastName = "LastNameTest",
                    UserName = "UserNameTest",
                    Email = "admin@indq.com",
                    Gender="M"
                    

                };
              await  userManager.CreateAsync(objectUser, "Password123$");
            }

        }
    }
}
