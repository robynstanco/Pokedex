using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pokedex.Data;
using Pokedex.Repository.Interfaces;
using Pokedex.Repository;
using Pokedex.Logging.Interfaces;
using Pokedex.Logging.Adapters;
using PokedexAPI.Interfaces;
using PokedexAPI.Logic;
using Pokedex.Common.Interfaces;
using Pokedex.Common.Helpers;

namespace PokedexAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private string POKEDEXDBConnectionString
        {
            get
            {
                return Configuration.GetConnectionString("POKEDEXDB_CONNECTION_STRING");
            }
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddCloudscribePagination();
            
            services.AddDbContext<POKEDEXDBContext>(op => op.UseSqlServer(POKEDEXDBConnectionString));

            services.AddSingleton(typeof(ILoggerAdapter<>), typeof(LoggerAdapter<>));
            services.AddScoped<IPaginationHelper, PaginationHelper>();
            services.AddScoped<IPokedexRepository, PokedexRepository>();
            services.AddScoped<IPokedexAPILogic, PokedexAPILogic>();

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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}