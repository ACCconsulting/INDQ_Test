using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aplicacion.Seguridad;
using Dominio;
using Dominio.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
   
    public class UsersController : MiBaseControllerController
    {
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(Login.Ejecuta data)
        {
            return await Mediator.Send(data);
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> Register(Register.Ejecuta data)
        {
            return await Mediator.Send(data);
        }
    }
}