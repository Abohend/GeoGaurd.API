
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
            builder.Services.Configure<GeoLocationOptions>(builder.Configuration.GetSection(GeoLocationOptions.SectionName));

            builder.Services.AddSingleton<ICountryCatalogService, CountryCatalogService>();
            builder.Services.AddSingleton<ICountryBlockRepository, InMemoryCountryBlockRepository>();
            builder.Services.AddSingleton<IBlockedAttemptsLogRepository, InMemoryBlockedAttemptsLogRepository>();

            builder.Services.AddSingleton<ICountryBlockService, CountryBlockService>();
            builder.Services.AddSingleton<IBlockedAttemptLogService, BlockedAttemptLogService>();
            builder.Services.AddSingleton<IIpAddressResolver, IpAddressResolver>();

            builder.Services.AddHttpClient<IGeoLocationService, GeoLocationService>((sp, httpClient) =>
            {
                var options = sp.GetRequiredService<IConfiguration>()
                    .GetSection(GeoLocationOptions.SectionName)
                    .Get<GeoLocationOptions>() ?? new GeoLocationOptions();

                httpClient.BaseAddress = new Uri(options.BaseUrl.TrimEnd('/') + "/");
            });

            builder.Services.AddHostedService<TemporalBlockCleanupService>();

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
