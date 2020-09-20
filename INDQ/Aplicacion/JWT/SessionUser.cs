using Aplicacion.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Aplicacion.JWT
{
    public class SessionUser: ISessionUser
    {
        private readonly IHttpContextAccessor _ihttpcontexAcesor;
        public SessionUser(IHttpContextAccessor ihttpcontexAcesor)
        {
            this._ihttpcontexAcesor = ihttpcontexAcesor;
        }

        public ClaimsPrincipal GetSessionUserId()
        {
            var UserId = _ihttpcontexAcesor.HttpContext.User;
   

            return UserId;

        }

        public string GetSessionUserString()
        {

            var UserName = _ihttpcontexAcesor.HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            

            return UserName;
        }
    }
}
