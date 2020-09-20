using Aplicacion.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Middleware
{
    public class HandlereErrorMeddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<HandlereErrorMeddleware> _logger;
        public HandlereErrorMeddleware(RequestDelegate next, ILogger<HandlereErrorMeddleware> logger)
        {
            this._next = next;
            this._logger = logger;
        }
        //Tiene que ser el nombre Inoke si no truena
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandlereExceptionAsync(context, ex, _logger);
            }

        }

        private async Task HandlereExceptionAsync(HttpContext context, Exception ex, ILogger<HandlereErrorMeddleware> logger)
        {
            object errores = null;
            switch (ex)
            {
                case HandlereException me:
                    {
                        logger.LogError(ex, "Manejador Error");
                        errores = me.Errores;
                        context.Response.StatusCode = (int)me.Codigo;
                        break;
                    }
                case Exception e:
                    {
                        logger.LogError(e, "Error de servidor");
                        errores = string.IsNullOrWhiteSpace(e.Message) ? "Error" : e.Message;
                        context.Response.StatusCode = 500;
                        break;
                    }
                default:
                    break;
            }
            context.Response.ContentType = "application/json";
            if (errores != null)
            {
                var result = JsonConvert.SerializeObject(new { errores });
                await context.Response.WriteAsync(result);
            }
        }
    }
}
