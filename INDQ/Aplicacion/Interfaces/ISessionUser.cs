using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Aplicacion.Interfaces
{
    public interface ISessionUser
    {
        ClaimsPrincipal GetSessionUserId();
        string GetSessionUserString();
    }
}
