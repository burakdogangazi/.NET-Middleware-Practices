using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Middleware.Middlewares;

namespace Middleware
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
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Middleware", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //.NET middlewares starts with Use and the order between middlewares is important.
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Middleware v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            //app.Run()
            app.Run(async context => Console.WriteLine("Middleware 1"));
            app.Run(async context => Console.WriteLine("Middleware 2"));
            //Middleware short-circuits -- after the run method nothing work below run command like Middleware2

            //app.Use()
            /*app.Use(async (context, next) =>
            {
                Console.WriteLine("Middleware 1 just started");
                await next.Invoke();
                Console.WriteLine("Middleware 1 finished");
            });
            app.Use(async (context, next) =>
            {
                Console.WriteLine("Middleware 2 just started");
                await next.Invoke();
                Console.WriteLine("Middleware 2 finished");
            });
            app.Use(async (context, next) =>
            {
                Console.WriteLine("Middleware 3 just started");
                await next.Invoke();
                Console.WriteLine("Middleware 3 finished");
            });*/
            /* Console-Output
             * Middleware 1 just started
             * Middleware 2 just started
             * Middleware 3 just started
             * Middleware 3 finished
             * Middleware 2 finished
             * Middleware 1 finished
             */

            /*app.Use(async (context, next) =>
            {
                Console.WriteLine("Use Middleware triggered");
                await next.Invoke();

            });

            app.Map("/example", internalApp => internalApp.Run(async context =>
            {
                Console.WriteLine(("/example middleware triggered"));
                await context.Response.WriteAsync(("/example middleware triggered"))
            }));*/

            /* Console-Output
             * Use Middleware triggered
             * /example middleware triggered
             */

            app.UseHello();
            // app.MapWhen()
            app.MapWhen(x => x.Request.Method == "GET", internalApp =>
            {
                internalApp.Run(async context =>
                {
                    Console.WriteLine("MapWhen middleware triggered");
                    await context.Response.WriteAsync("triggered");
                });
            });//Output in response not in console.

            

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}