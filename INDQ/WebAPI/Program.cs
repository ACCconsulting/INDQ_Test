using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dominio;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistencia;

namespace WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //CreateHostBuilder(args).Build().Run();

            var hostServer = CreateHostBuilder(args).Build();
            using(var enviroment = hostServer.Services.CreateScope())
            {
                var services = enviroment.ServiceProvider;
                try
                {
                    var userManagger = services.GetRequiredService<UserManager<User>>();
                    var context = services.GetRequiredService<IndqContext>();
                    context.Database.Migrate();
                    //Insert user by Default
                    DefaultUser.InsertData(context, userManagger).Wait();

                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex,"Ocurrio un error en la migracion");

                }
               
            }

            hostServer.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
