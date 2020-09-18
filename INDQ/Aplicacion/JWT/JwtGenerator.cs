using Aplicacion.Interfaces;
using Dominio;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Aplicacion.JWT
{
   public class JwtGenerator : IJwtGenerator
    {
        public string CreateToken(User User)
        {
            var claims =new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId,User.UserName)
            };

            //Clave,  la clave debe ser mayor a 8
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Mi palabra secreta"));
            //Credenciales
            var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            //Descripcion del token
            var tokenDescripcion = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credenciales

            };
            //Manejador
            var handlerToken = new JwtSecurityTokenHandler();
            var token = handlerToken.CreateToken(tokenDescripcion);

            //DEvolver el token
            return handlerToken.WriteToken(token);
        }
    }
}
