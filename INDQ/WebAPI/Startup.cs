using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aplicacion.Interfaces;
using Aplicacion.JWT;
using Aplicacion.Seguridad;
using Dominio;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.Generation.Processors.Security;
using Persistencia;
using WebAPI.Middleware;

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
                opt.UseSqlServer(conect,x=>x.UseNetTopologySuite());
            });

            services.AddControllers(opt=> {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                opt.Filters.Add(new AuthorizeFilter(policy));
            }).AddFluentValidation(cfg =>cfg.RegisterValidatorsFromAssemblyContaining<LoginUser>());

            services.AddMediatR(typeof(LoginUser.Login).Assembly);



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

            //Inyectar clase para obtener usuario en session
            services.AddScoped<ISessionUser, SessionUser>();


            //No permita hacer peticiones sin token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Mi palabra secreta"));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateAudience = false,
                    ValidateIssuer = false    //solamente se le envie a ciertos ip false
                };
            });



            /// // REGISTRAMOS SWAGGER COMO SERVICIO
            services.AddOpenApiDocument(document =>
            {
                document.Title = "INDQ API Events";
                document.Description = "API de una aplicacion intuitiva que permite registrarse como usuario, iniciar sesión, acceder a un listado de eventos, mostrar su detalle, confirmar asistencia, calificar el evento, buscar eventos cercanos a ti o por dirección";

                // CONFIGURAMOS LA SEGURIDAD JWT PARA SWAGGER,
                // PERMITE AÑADIR EL TOKEN JWT A LA CABECERA.
                document.AddSecurity("JWT", Enumerable.Empty<string>(),
                    new OpenApiSecurityScheme
                    {
                        Type = OpenApiSecuritySchemeType.ApiKey,
                        Name = "Authorization",
                        In = OpenApiSecurityApiKeyLocation.Header,
                        Description = "Copia y pega el Token en el campo 'Value:' así: Bearer {Token JWT}."
                    }
                );

                document.OperationProcessors.Add(
                    new AspNetCoreOperationSecurityScopeProcessor("JWT"));
            });
            //services.AddSwaggerGen(conf =>
            //{
            //    conf.SwaggerDoc("v1", new OpenApiInfo
            //    {
            //        Title = "INDQ TEST",
            //        Version = "v1"

            //    });
            //    conf.CustomSchemaIds(e => e.FullName);
            //});





        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<HandlereErrorMeddleware>();

            if (env.IsDevelopment())
            {
               //app.UseDeveloperExceptionPage();
            }
            //Descomentar cunado se pase a produccion
            //app.UseHttpsRedirection();

            app.UseOpenApi();
            app.UseSwaggerUi3();
            //app.UseSwagger(c =>
            //{
            //    c.RouteTemplate = "INDQ/swagger/{documentName}/swagger.json";
            //});
            ////Para la interfaz
            //app.UseSwaggerUI(c =>
            //{
            //    c.SwaggerEndpoint("/INDQ/swagger/v1/swagger.json", "TestService");
            //});

            //app.UseSwaggerUI(conf => {
            //    conf.SwaggerEndpoint("swagger/v1/swagger.json", "INDQ TEST V1");
            //});

            app.UseRouting();
            // Se activa cuadno se configura el token  services.AddAuthentication()
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
           
        }
    }
}
