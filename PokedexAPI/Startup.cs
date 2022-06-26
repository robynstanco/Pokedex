using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pokedex.Common;
using Pokedex.Data;
using Pokedex.Logging.Adapters;
using Pokedex.Logging.Interfaces;
using Pokedex.Repository;
using Pokedex.Repository.Interfaces;
using PokedexAPI.Interfaces;
using PokedexAPI.Logic;
using System;

namespace PokedexAPI
{
    /// <summary>
    /// The Startup class.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// The POKEDEXDB Connection String.
        /// </summary>
        private string POKEDEXDBConnectionString
        {
            get
            {
                return Configuration.GetConnectionString("POKEDEXDB_CONNECTION_STRING");
            }
        }

        /// <summary>
        /// The Azure Application Insights Connection String.
        /// </summary>
        private string ApplicationInsightsConnectionString
        {
            get
            {
                return Configuration["APPINSIGHTS_CONNECTIONSTRING"];
            }
        }

        public IConfiguration Configuration { get; }

        public ILogger Logger { get; set; }

        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Configure the service collection with db context, service dependencies, application insights, and health check.  
        /// </summary>
        /// <param name="services">Service collection to configure.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            Logger.LogInformation($"{Constants.Added} Controllers.");

            services.AddDbContext<POKEDEXDBContext>(op => op.UseSqlServer(POKEDEXDBConnectionString));
            Logger.LogInformation($"{Constants.Added} {Constants.Pokemon} {Constants.DBContext}.");

            services.AddSingleton(typeof(ILoggerAdapter<>), typeof(LoggerAdapter<>));
            services.AddScoped<IPokedexRepository, PokedexRepository>();
            services.AddScoped<IPokedexAPILogic, PokedexAPILogic>();
            Logger.LogInformation($"{Constants.Added} Dependency Injection for automapper, custom logging, logic, helpers, and repository.");

            /*services.AddApplicationInsightsTelemetry(ApplicationInsightsConnectionString);
            Logger.LogInformation($"{Constants.Added} Application Insights.");

            services.AddHealthChecks().AddDbContextCheck<POKEDEXDBContext>();
            Logger.LogInformation($"{Constants.Added} HealthChecks.");*/ //todo

            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My PokedexAPI V1");
            });

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            Logger.LogInformation("Configured Application.");
        }
    }
}