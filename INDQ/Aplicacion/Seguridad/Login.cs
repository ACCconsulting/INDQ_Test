using Aplicacion.Interfaces;
using Dominio;
using Dominio.Dto;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Seguridad
{
  public  class Login
    {
        public class Ejecuta : IRequest<UserDto>
        {
            //public string FirstName { get; set; }
            //public string LastName { get; set; }
            public string Email { get; set; }
            //public string Gender { get; set; }
            public string Password { get; set; }
        }

        public class ValidationExecute: AbstractValidator<Ejecuta>
        {
            public ValidationExecute()
            {
                RuleFor(x => x.Email).NotEmpty();
                RuleFor(x => x.Password).NotEmpty();
            }
        }

        public class Manejador : IRequestHandler<Ejecuta, UserDto>
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


            public  async Task<UserDto> Handle(Ejecuta request, CancellationToken cancellationToken)
            {

                var objUser = await _userManager.FindByEmailAsync(request.Email);
                if(objUser == null)
                {
                    throw new Exception(System.Net.HttpStatusCode.Unauthorized.ToString());
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

                throw new Exception("No esta autorizado");
            }
        }

       
    }
}
