using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TempLoggerService.Api.Repositories;

namespace TempLoggerService.Api
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

            var connectionString = Configuration.GetConnectionString("Database");
            services.AddDbContext<ApiContext>(options => options.UseSqlServer(connectionString));

            string dashboardServer = Configuration.GetSection("DashboardServer").Value;

            services.AddScoped<ITemperatureRepository, TemperatureRepository>();
            services.AddScoped<IDeviceRepository, DeviceRepository>();

            services.AddCors(options => {
                options.AddPolicy("devAllowAll", builder => {
                    builder.AllowAnyOrigin();
                });

                options.AddPolicy("local", builder => {
                    // Allow the prod API to accept requests from a dev frontend
                    builder.WithOrigins("http://localhost:5010");
                });

                if (!string.IsNullOrWhiteSpace(dashboardServer))
                {
                    options.AddPolicy("prod", builder => {
                        // Allow the API to accept requests from the configured dashboard server.
                        builder.WithOrigins(dashboardServer);
                    });
                }
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            if (env.IsDevelopment())
            {
                app.UseCors("devAllowAll");
            }
            else
            {
                app.UseCors("prod");
            }

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
