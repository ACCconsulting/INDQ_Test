using Dominio;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Interfaces
{
   public interface IJwtGenerator
    {
        string CreateToken(User User);
    }
}
