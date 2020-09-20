using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aplicacion.Crud.Events;
using Dominio.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Todo sobre Eventos
    /// </summary>
    public class EventsController : MiBaseControllerController
    {
        /// <summary>
        /// Consultar Listao de Eventos
        /// </summary>
        /// <param name="page">Pagina Actual</param>
        /// <param name="limit">Registros por pagina</param>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="title">Titulo del Evento</param>
        /// <param name="distancia">Distancia Agregar mayor a 0  para obtener mas registros</param>
        /// <returns></returns>
        [HttpGet]
       
       
        public async Task<PageResult<EventsDto>> GetAll(int? page, int? limit, decimal? lat, decimal? lng, string title, double distancia)
        {
            return await Mediator.Send(new EventsList.Ejecuta { Page = page, Limit = limit, Lat = lat, lng = lng, Title = title,Distancia= distancia });
        }

        [HttpPost]
        public async Task<ActionResult<string>> Insert(Insert.Event data)
        {
            return await Mediator.Send(data);
        }
        /// <summary>
        /// Marcar asistencia a evento
        /// </summary>
        /// <param name="eventId">ID del evento</param>
        /// <returns></returns>
        [HttpPost()]
        [Route("/attendance/{eventId}")]
        public async Task<ActionResult<AttendanceUserDto>> Attattendance(int eventId)
        {
            return await Mediator.Send(new AttendanceUser.Ejecuta { EventId= eventId });
        }

        /// <summary>
        /// Desmarca asistencia a un evento para el usuario actual
        /// </summary>
        /// <param name="eventId">ID del evento para desmarcar la asistencia</param>
        /// <returns></returns>
        [HttpDelete()]
        [Route("/attendance/{eventId}")]
        public async Task<ActionResult<AttendanceUserDto>> AttattendanceDelete(int eventId)
        {
            return await Mediator.Send(new AttendenceDelete.Ejecuta { EventId = eventId });
        }
    }
}