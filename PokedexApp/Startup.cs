using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pokedex.Data.Models;
using Pokedex.Logging.Adapters;
using Pokedex.Logging.Interfaces;
using Pokedex.Repository.Interfaces;
using Pokedex.Repository.Repositories;
using PokedexApp.Interfaces;
using PokedexApp.Logic;

namespace PokedexApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddDbContext<POKEDEXDBContext>(op => op.UseSqlServer(
                Configuration.GetConnectionString("POKEDEXDB_CONNECTION_STRING")));

            services.AddSingleton(typeof(ILoggerAdapter<>), typeof(LoggerAdapter<>));

            services.AddScoped<IPokedexAppLogic, PokedexAppLogic>();
            services.AddScoped<IPokedexRepository, PokedexRepository>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Pokedex/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Pokedex}/{action=Index}");
            });
        }
    }
}