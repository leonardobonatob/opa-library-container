using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// Import of opa middleware
using opa_library;

namespace web_app_example
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Configure usage of opa to avoid healthcheck
            app.UseWhen(
                context => !(context.Request.Path == "/healthcheck"),
                appBuilder => HandleOpa(appBuilder)
            );

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDelete("/", async context => {
                    await context.Response.WriteAsync("Delete works!");
                });
                
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("GET works!");
                });

                // Healthcheck route
                endpoints.MapGet("/healthcheck", async context =>
                {
                    context.Response.StatusCode = 200;
                    await context.Response.WriteAsync("OK!");
                });

                endpoints.MapPost("/", async context => {
                    await context.Response.WriteAsync("Post works!");
                });

                endpoints.MapPut("/", async context => {
                    await context.Response.WriteAsync("Put works!");
                });
            });
        }

        private void HandleOpa(IApplicationBuilder app){
            // Use of Opa Middleware
            app.UseRequestOpa();
        }
    }
}
