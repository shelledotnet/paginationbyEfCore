using EfCoreDb.Domain.Context;
using EfCoreDb.Domain.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EfCoreDb.API
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

     
            services.AddControllers(options =>
            {

                #region Adding Filter Globally that affect all controllers
                options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status200OK));
                options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status201Created));
                options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status204NoContent));
                options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status400BadRequest));
                options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status401Unauthorized));
                options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status403Forbidden));
                options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status404NotFound));
                options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status405MethodNotAllowed));
                options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status406NotAcceptable));
                options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status409Conflict));
                options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status500InternalServerError));
                options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status503ServiceUnavailable));
                //options.Filters.Add<Version1DiscontinueResourceFilter>();
                //options.Filters.Add<DtoStateValidatorActionFilter>();
                #endregion

                //which tells the server that if the client tries to negotiate for the Accept header media type the server doesn’t support
                //, it should return the 406 Not Acceptable status code.
                options.ReturnHttpNotAcceptable = true;

               // options.OutputFormatters.Add(new XmlSerializerOutputFormatter());

            })
            //for custom response on the model
            .ConfigureApiBehaviorOptions(options =>
            {

                options.SuppressModelStateInvalidFilter = true;
            });
            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                Configuration.GetConnectionString("EfConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
            services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "EfCoreDb.API", Version = "v1" });
            });
            #region Configuring The Service To Get The Base URL

            #endregion
            services.AddHttpContextAccessor();
            services.AddSingleton<IUriService>(o =>
            {
                var accessor = o.GetRequiredService<IHttpContextAccessor>();
                var request = accessor.HttpContext.Request;
                var uri = string.Concat(request.Scheme, "://", request.Host.ToUriComponent());
                return new UriService(uri);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "EfCoreDb.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
