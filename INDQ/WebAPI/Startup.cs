using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aplicacion.Interfaces;
using Aplicacion.JWT;
using Aplicacion.Seguridad;
using Dominio;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistencia;

namespace WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // dependency Injection context,set conection string form app.setting
            services.AddDbContext<IndqContext>(opt=>
            {
                var conect = Configuration.GetSection("ConnectionString").GetValue<string>("DefaultConection");
                opt.UseSqlServer(conect);
            });

            services.AddControllers().AddFluentValidation(cfg =>cfg.RegisterValidatorsFromAssemblyContaining<Login>());

            services.AddMediatR(typeof(Login.Manejador).Assembly);



            //cunfuguracion que necesita el identity para trabajar dentro del api---------------------

            var builder = services.AddIdentityCore<User>();
            var identityBuilder = new IdentityBuilder(builder.UserType, builder.Services);
            //para traajar con los roles
            identityBuilder.AddRoles<IdentityRole>();
            //Para incluir data de roles dentro de los token de seguridad
            identityBuilder.AddClaimsPrincipalFactory<UserClaimsPrincipalFactory<User, IdentityRole>>();
            // 

            ////contexto
            identityBuilder.AddEntityFrameworkStores<IndqContext>();
          
            identityBuilder.AddSignInManager<SignInManager<User>>();

            //-------------------------------

            // usuario default identity
            services.TryAddSingleton<ISystemClock, SystemClock>();

            //Injection JWT
            services.AddScoped<IJwtGenerator, JwtGenerator>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //Descomentar cunado se pase a produccion
            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
