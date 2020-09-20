using Aplicacion.Helpers;
using Aplicacion.Interfaces;
using Dominio;
using Dominio.Dto;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Persistencia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Crud.Events
{
    public class AttendanceUser
    {
        public class Ejecuta : IRequest<AttendanceUserDto>
        {
            public int EventId { get; set; }
        }

        public class ValidationExecute : AbstractValidator<Ejecuta>
        {
            public ValidationExecute()
            {
                RuleFor(x => x.EventId).NotEmpty();
            }
        }

        public class Manejador : IRequestHandler<Ejecuta, AttendanceUserDto>
        {
            private readonly IndqContext _indqContext;
            private readonly UserManager<User> _userManager;
            private readonly ISessionUser _sessionUser;
            public Manejador(IndqContext indqContext, UserManager<User> userManager, ISessionUser sessionUser)
            {
                this._indqContext = indqContext;
                this._userManager = userManager;
                this._sessionUser = sessionUser;

            }
            public async Task<AttendanceUserDto> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                  var CurrenUser= await  _userManager.FindByNameAsync(this._sessionUser.GetSessionUserString());

                    var ExisteEvento = _indqContext.Events.Any(c => c.EventsId == request.EventId);
                    if(!ExisteEvento) throw new HandlereException(HttpStatusCode.NotFound, new { Error = "Evento no encontrado" });

                var thereAreAttendance = _indqContext.Attendances.Any(c => c.EventsId == request.EventId && c.UserId==CurrenUser.Id);
                if (thereAreAttendance) throw new HandlereException(HttpStatusCode.NotFound, new { Error = "Ya esta registrado en este Evento" });

                var newAttendance = new Attendance
                    {
                        EventsId = request.EventId,
                        UserId = CurrenUser.Id
                    };
                try
                {
                     this._indqContext.Attendances.Add(newAttendance);
                    var Result = await _indqContext.SaveChangesAsync();

                    if (Result > 0)
                    {
                        var attendanceCount = _indqContext.Attendances.Where(x => x.EventsId == request.EventId).Count();
                        return new AttendanceUserDto
                        {
                            Id = newAttendance.EventsId,
                            Attendances = attendanceCount
                        };
                    }
                }
                catch (Exception ex)
                {

                    throw new HandlereException(HttpStatusCode.BadRequest, new { Error = "Error al registrar la asistencia al evento" });
                }


                throw new HandlereException(HttpStatusCode.BadRequest, new { Error = "Error al registrar la asistencia al evento" });

               

                
               

                
            }
        }
    }
}
