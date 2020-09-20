using Aplicacion.Helpers;
using Aplicacion.Interfaces;
using Dominio;
using Dominio.Dto;
using FluentValidation;
using MediatR;
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
   public class AttendenceDelete
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
            private readonly IndqContext _indqContex;
            private readonly UserManager<User> _userManager;
            private readonly ISessionUser _sessionUser;

            public Manejador(IndqContext indqContext, UserManager<User> userManager, ISessionUser sessionUser)
            {
                this._indqContex = indqContext;
                this._userManager = userManager;
                this._sessionUser = sessionUser;

            }
            public async Task<AttendanceUserDto> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var CurrenUser = await _userManager.FindByNameAsync(this._sessionUser.GetSessionUserString());

                var ExisteEvento = _indqContex.Events.Any(c => c.EventsId == request.EventId);
                if (!ExisteEvento) throw new HandlereException(HttpStatusCode.NotFound, new { Error = "Evento no encontrado" });

                var AttendenceDelete =  _indqContex.Attendances.Where(e=>e.EventsId==request.EventId && e.UserId==CurrenUser.Id).FirstOrDefault();
                if (AttendenceDelete == null) throw new HandlereException(HttpStatusCode.NotFound, new { Error = "Asistencia no registrada anteriormente" });

                this._indqContex.Remove(AttendenceDelete);
                var result = await this._indqContex.SaveChangesAsync();
                if (result > 0)
                {
                    var attendanceCount = _indqContex.Attendances.Where(x => x.EventsId==request.EventId).Count();
                    return new AttendanceUserDto
                    {
                        Id = AttendenceDelete.AttendanceId,
                        Attendances = attendanceCount

                    };
                }
                throw new HandlereException(HttpStatusCode.BadRequest, new { Error = "Error al eliminar la asistencia al evento" });

            }
        }
    }
}
