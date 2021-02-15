using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pokedex.Common;
using Pokedex.Common.Helpers;
using Pokedex.Common.Interfaces;
using Pokedex.Data;
using Pokedex.Logging.Adapters;
using Pokedex.Logging.Interfaces;
using Pokedex.Repository;
using Pokedex.Repository.Interfaces;
using PokedexApp.Interfaces;
using PokedexApp.Logic;

namespace PokedexApp
{
    public class Startup
    {
        private string POKEDEXDBConnectionString
        {
            get
            {
                return Configuration.GetConnectionString("POKEDEXDB_CONNECTION_STRING");
            }
        }

        private string ApplicationInsightsConnectionString
        {
            get
            {
                return Configuration["APPINSIGHTS_CONNECTIONSTRING"];
            }
        }

        public IConfiguration Configuration { get; set; }
        public ILogger Logger { get; set; }
        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            Configuration = configuration;
            Logger = logger;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddCloudscribePagination();
            Logger.LogInformation(Constants.Added + " MVC & Pagination.");

            services.AddDbContext<POKEDEXDBContext>(op => op.UseSqlServer(POKEDEXDBConnectionString));
            Logger.LogInformation(Constants.Added + " " + Constants.Pokemon + Constants.DBContext + ".");

            services.AddSingleton(typeof(ILoggerAdapter<>), typeof(LoggerAdapter<>));
            services.AddScoped<IPaginationHelper, PaginationHelper>();
            services.AddScoped<IPokedexAppLogic, PokedexAppLogic>();
            services.AddScoped<IPokedexRepository, PokedexRepository>();
            Logger.LogInformation(Constants.Added + " Dependency Injection for custom logging, logic, helpers, and repository.");

            services.AddApplicationInsightsTelemetry(ApplicationInsightsConnectionString);
            Logger.LogInformation(Constants.Added + " Application Insights.");

            services.AddHealthChecks().AddDbContextCheck<POKEDEXDBContext>();
            Logger.LogInformation(Constants.Added + "HealthChecks.");
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/" + Constants.Error);
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=" + Constants.PokedexNoAccent + "}/{action=Index}");

                endpoints.MapHealthChecks("/health");
            });

            Logger.LogInformation("Configured Application.");
        }
    }
}