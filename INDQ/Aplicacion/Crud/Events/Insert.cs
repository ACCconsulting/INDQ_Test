using Aplicacion.Helpers;
using Dominio;
using FluentValidation;
using MediatR;
using NetTopologySuite;
using NetTopologySuite.Geometries;
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
   public class Insert
    {
        public class Event : IRequest<string>
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public DateTime Date { get; set; }
            public string Imagen { get; set; }
            public int Attendances { get; set; }
            public bool WillYouAttend { get; set; }
            public decimal[] Location { get; set; }
          


        }


        public class ValidationExecute : AbstractValidator<Event>
        {
           
            public ValidationExecute()
            {
                RuleFor(x => x.Title).NotEmpty();
                RuleFor(x => x.Description).NotEmpty();
                RuleFor(x => x.Date).NotEmpty();
                RuleFor(x => x.Imagen).NotEmpty();
                RuleFor(x => x.Attendances).NotEmpty();
                //RuleFor(x => x.WillYouAttend).NotNull();
                RuleFor(x => x.Location).NotEmpty();

            }
        }

        public class Manejador : IRequestHandler<Event, string>
        {
            private readonly IndqContext _context;
            public Manejador(IndqContext context)
            {
                this._context = context;
            }
            public async Task<string> Handle(Event request, CancellationToken cancellationToken)
            {
               
                    if (request.Date<DateTime.Now) throw new HandlereException(HttpStatusCode.BadRequest, new { Error = "La fecha del evento no debe ser menor al día de hoy. " });

                    var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                    double lng = double.Parse(request.Location[0].ToString()); //X
                    double Lat = double.Parse(request.Location[1].ToString()); //Y

                    var Events = new Dominio.Events
                    {
                        Title = request.Title,
                        Description = request.Description,
                        Date = request.Date,
                        Imagen = request.Imagen,
                        Attendances = request.Attendances,
                        WillYouAttend = request.WillYouAttend,
                        Location = geometryFactory.CreatePoint(new Coordinate(Lat,lng))

                    };

                    this._context.Events.Add(Events);
                    var Result = await this._context.SaveChangesAsync();
                    if (Result > 0)
                        return Events.EventsId.ToString();


                     throw new HandlereException(HttpStatusCode.InternalServerError, new { Error = "Ocurrio un erro al registrar los eventos" });

                   
                
               
                
               
            }
        }


    }
}
