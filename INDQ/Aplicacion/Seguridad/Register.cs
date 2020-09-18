using Dominio;
using Dominio.Dto;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistencia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Seguridad
{
    public class Register
    {
        public class Ejecuta : IRequest<UserDto>
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Gender { get; set; }
            public string Password { get; set; }
        }


        public class Manejador : IRequestHandler<Ejecuta, UserDto>
        {
            private readonly IndqContext _context;
            private readonly UserManager<User> _userManager;
            //private readonly IJwtGenerator _ijwtGenerator;
            public Manejador(IndqContext contex, UserManager<User> userManager)
            {
                _context = contex;
                _userManager = userManager;
                //_ijwtGenerator = jwtGenerator;
            }

            public class ExecuteValidation: AbstractValidator<Ejecuta>
            {
                public ExecuteValidation()
                {
                    RuleFor(x => x.FirstName).NotEmpty();
                    RuleFor(x => x.LastName).NotEmpty();
                    RuleFor(x => x.Email).NotEmpty();
                    RuleFor(x => x.Password).NotEmpty();
                    RuleFor(x => x.Gender).NotEmpty();
                }
            }

            public async Task<UserDto> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var existeEmail = await _context.Users.Where(e => e.Email == request.Email).AnyAsync();

                if (existeEmail)
                {
                    //throw new ManejadorExcepcion(System.Net.HttpStatusCode.BadRequest, new { Mensaje = "Existe un usuario registrado con este email" });
                    //se tiene que regresar el codigo 403
                    throw new Exception("Usuario ya registrado con ese correo");
                }

                var User = new User
                {
                    FirstName = request.FirstName,
                    LastName  = request.LastName,
                    Email = request.Email,
                    UserName = request.Email,
                    Gender  = request.Gender
                };
                var result = await _userManager.CreateAsync(User, request.Password);


                if (result.Succeeded)
                {

                    return new UserDto
                    {
                        FirstName = User.FirstName,
                        //Token = _ijwtGenerator.CreateToken(usuario, null),
                        LastName = User.LastName,
              
                    };
                }

                throw new Exception("Ocurrio un Error al Crearse el usuario");


            }
        }
    }
}
