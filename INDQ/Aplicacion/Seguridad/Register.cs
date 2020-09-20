using Aplicacion.Helpers;
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
        public class Users : IRequest<UserDto>
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Gender { get; set; }
            public string Password { get; set; }
        }

        public class ValidationExecute : AbstractValidator<Users>
        {
           
            private bool BeAValidPostcode(string FirstName)
            {
                int valor = 0;
                return !int.TryParse(FirstName, out valor);
            }

            public ValidationExecute()
            {
                RuleFor(x => x.FirstName).Must(BeAValidPostcode).WithMessage("Solo acepta letras");
                RuleFor(x => x.LastName).NotEmpty();
                RuleFor(x => x.Email).NotEmpty().EmailAddress() ;
                RuleFor(x => x.LastName).NotEmpty();
                RuleFor(x => x.Password).NotEmpty().MinimumLength(8).WithMessage("Password debe ser mayor o igual que 8 caracteres, Una mayuscula y un caracter especial");
            }
           
        }
        public class Manejador : IRequestHandler<Users, UserDto>
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

         

            public async Task<UserDto> Handle(Users request, CancellationToken cancellationToken)
            {
                var existeEmail = await _context.Users.Where(e => e.Email == request.Email).AnyAsync();

                if (existeEmail)
                {
                    throw new HandlereException(System.Net.HttpStatusCode.Forbidden, new { Mensaje = "La cuenta con ese correo electrónico ya existe" });
        
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

                throw new HandlereException(System.Net.HttpStatusCode.InternalServerError, new { Mensaje = "Ocurrio un erro al registar los datos" });


            }
        }
    }
}
