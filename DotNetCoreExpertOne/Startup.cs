using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreExpertOne
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            app.UseRouting();

            app.UseAuthorization();

            app.UseStaticFiles();

            app.Use(async (ctx, next) =>
            {
                var timer = Stopwatch.StartNew();
                logger.LogInformation($"Request started on : {env.EnvironmentName}");
                await next();
                var eclipsedTime = timer.ElapsedMilliseconds;
                logger.LogInformation($"Request end in {eclipsedTime}ms");
            });


            app.MapWhen(ctx => ctx.Request.Headers["User-Agent"].First().Contains("Chrome"), ChromeHandler);

            app.Map("/index", a => {
                a.Use(async (c, n) => {
                    c.Response.ContentType = "text/html";
                    await c.Response.WriteAsync("<h1>This is my index page (map middle ware) </h1>");
                });
            });
            app.Run(async ctx =>
            {
                ctx.Response.ContentType = "text/html";
                await ctx.Response.WriteAsync("this is Terminating middle ware (run) !!");
            });
        }
        private void ChromeHandler(IApplicationBuilder app)
        {
            app.Use(async (c, n) =>
            {
                c.Response.ContentType = "text/html";
                await c.Response.WriteAsync("<h1>its chrome broswer </h1>");
            });
        }
    }
}
