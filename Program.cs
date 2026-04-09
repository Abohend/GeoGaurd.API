
using GeoGaurd.API.Middleware;
using GeoGaurd.API.Models.Config;
using GeoGaurd.API.Repositories;
using GeoGaurd.API.Services;

namespace GeoGaurd.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSingleton<ICountryCatalogService, CountryCatalogService>();
            builder.Services.AddSingleton<ICountryBlockRepository, InMemoryCountryBlockRepository>();

            builder.Services.AddSingleton<ICountryBlockService, CountryBlockService>();
            // Add services to the container.
            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();
            
            app.UseGlobalExceptionMiddleware();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
