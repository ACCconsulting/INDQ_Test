using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aplicacion.Seguridad;
using Dominio;
using Dominio.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Operaciones de usuarios
    /// </summary>
    [AllowAnonymous]
 
    public class UsersController : MiBaseControllerController
    {

        /// <summary>
        /// Inicio de sesión  
        /// </summary>
        /// <param name="data">Usuario a registrar</param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginUser.Login data)
        {
            return await Mediator.Send(data);
        }

        /// <summary>
        /// Registro de usuario
        /// </summary>
        /// <param name="Register">Credenciales de acceso</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<UserDto>> Register(Register.Users Register)
        {
            return await Mediator.Send(Register);
        }
    }
}