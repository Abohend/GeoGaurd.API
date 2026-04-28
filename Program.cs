using GeoGaurd.API.BackgroundServices;
using GeoGaurd.API.Middleware;
using GeoGaurd.API.Models.Config;
using GeoGaurd.API.Repositories;
using GeoGaurd.API.Services;
using Microsoft.Extensions.Options;

namespace GeoGaurd.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
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
                var options = sp.GetRequiredService<IOptions<GeoLocationOptions>>().Value;
                httpClient.BaseAddress = new Uri(options.BaseUrl.TrimEnd('/') + "/");
                httpClient.DefaultRequestHeaders.Add("User-Agent", "GeoGuard.API/1.0");
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            })
            .AddStandardResilienceHandler(options =>
            {
                var resilienceConfig = builder.Configuration.GetSection("GeoLocationApi:Resilience");
                options.Retry.MaxRetryAttempts = resilienceConfig.GetValue("MaxRetryAttempts", 3);
                options.Retry.Delay = TimeSpan.FromSeconds(resilienceConfig.GetValue("RetryBaseDelaySeconds", 2.0));
                options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(resilienceConfig.GetValue("CircuitBreakerDurationSeconds", 30.0));
                options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(resilienceConfig.GetValue("TotalTimeoutSeconds", 60.0));
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
