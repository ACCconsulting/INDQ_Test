using MediatR;
using Persistencia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Dominio.Dto;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using Aplicacion.Helpers;
using System.Net;
using FluentValidation;

namespace Aplicacion.Crud.Events
{
    public class EventsList
    {
        public class Ejecuta : IRequest<PageResult<EventsDto>>
        {
            public int? Page { get; set; }
            public int? Limit { get; set; }
            public decimal? Lat { get; set; }
            public decimal? lng { get; set; }
            public string Title { get; set; }

            public double Distancia { get; set; }
        }


        public class ValidationExecute : AbstractValidator<Ejecuta>
        {

            public ValidationExecute()
            {
                RuleFor(x => x.Distancia).NotEqual(0).WithMessage("Distancia tiene que ser mayor a 0");
            }

        }
        public class Manejador : IRequestHandler<Ejecuta, PageResult<EventsDto>>
        {
            private readonly IndqContext _indqContext;
            public Manejador(IndqContext indqContext)
            {
                this._indqContext = indqContext;
            }
            public async Task<PageResult<EventsDto>> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                
                    List<Dominio.Events> objEvents = null;
                    int? Inicio = 0, Fin = 0;
                    Func<Dominio.Events, bool> predicate = x => true;
                    if (!string.IsNullOrEmpty(request.Title))
                        predicate = x => x.Title.Contains(request.Title);
                    else if (request.Lat != null || request.lng != null)
                    {

                        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

                        var currentLocation =
                            geometryFactory.CreatePoint(new Coordinate(double.Parse(request.lng.ToString()), double.Parse(request.Lat.ToString())));
                        //predicate = e => e.Location.IsWithinDistance(currentLocation,request.Distancia);
                        predicate = c => c.Location.Distance(currentLocation) < request.Distancia;


                    }
                   
                        objEvents = _indqContext.Events.Where(predicate).ToList();

                    #region  Paginado

                    int totalRows = objEvents.Count();
                    //int? totalRows = resultado.Count();
                    var PagesNumber = totalRows / request.Limit;

                    if ((totalRows % request.Limit) > 0)
                    {
                        PagesNumber += 1;
                    }
                    if (request.Page == 1)
                    {
                        Inicio = (request.Page * request.Limit) - request.Limit;
                        Fin = (request.Page * request.Limit);
                    }
                    else
                    {
                        Inicio = ((request.Page * request.Limit) - request.Limit) + 1;
                        Fin = (request.Page * request.Limit);

                    }

                    var executeQuery = objEvents.Where(predicate).Skip(int.Parse(Inicio.ToString())).Take(int.Parse(Fin.ToString()))
                                                .OrderByDescending(o => o.Date).ToList();

                    #endregion

                    var items = executeQuery.Select(e => new EventsDto
                    {
                        EventsId = e.EventsId,
                        Title = e.Title,
                        Description = e.Description,
                        Date = e.Date,
                        Imagen = e.Imagen,
                        Attendances = e.Attendances,
                        WillYouAttend = e.WillYouAttend
                    });

                    var Result = new PageResult<EventsDto>
                    {
                        Page = (int)request.Page,
                        Pages = (int)PagesNumber,
                        Items = items
                    };


                    return await Task.FromResult(Result);



                throw new HandlereException(HttpStatusCode.InternalServerError, new { Error = "Error al obtener los eventos " });



                
            }
        }
    }
}
