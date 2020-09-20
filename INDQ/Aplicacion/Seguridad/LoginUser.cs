using Aplicacion.Helpers;
using Aplicacion.Interfaces;
using Dominio;
using Dominio.Dto;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Seguridad
{
  public  class LoginUser
    {
        public class Login : IRequest<UserDto>
        {
            /// <summary>
            /// Correo del usuario
            /// </summary>
            public string Email { get; set; }
         
            public string Password { get; set; }
        }

        public class ValidationExecute: AbstractValidator<Login>
        {
            public ValidationExecute()
            {
                RuleFor(x => x.Email).NotEmpty();
                RuleFor(x => x.Password).NotEmpty();
            }
        }

        public class Manejador : IRequestHandler<Login, UserDto>
        {
            private readonly UserManager<User> _userManager;
            private readonly SignInManager<User> _signInManager;
            private readonly IJwtGenerator _IJwtGenerator;
            
            public Manejador(Microsoft.AspNetCore.Identity.UserManager<User> userManager, SignInManager<User> signInManager, IJwtGenerator jwtGenerator)
            {
                this._userManager = userManager;
                this._signInManager = signInManager;
                this._IJwtGenerator = jwtGenerator;
            }


            public  async Task<UserDto> Handle(Login request, CancellationToken cancellationToken)
            {

                var objUser = await _userManager.FindByEmailAsync(request.Email);
                if(objUser == null)
                {
                    throw new HandlereException(HttpStatusCode.BadRequest, new { Error = "No se Encontro un usuario con ese correo" });
                    //throw new Exception(System.Net.HttpStatusCode.Unauthorized.ToString());
                }
                var result = await _signInManager.CheckPasswordSignInAsync(objUser, request.Password, false);
                if (result.Succeeded)
                {
                    return new UserDto
                    {
                        Id = objUser.Id,
                        FirstName = objUser.FirstName,
                        LastName = objUser.LastName,
                        Token = _IJwtGenerator.CreateToken(objUser)
                    };
                    //return objUser;
                }
                throw new HandlereException(HttpStatusCode.BadRequest, new { Error = "Credenciales inválidas"});

               
            }
        }

       
    }
}
