using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using SupportDesk.Infrastructure.Persistence;

namespace SupportDesk.IntegrationTests;

public sealed class SupportDeskApiFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"SupportDeskTests-{Guid.NewGuid():N}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();
        });
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<SupportDeskDbContext>>();
            services.RemoveAll<SupportDeskDbContext>();
            services.AddDbContext<SupportDeskDbContext>(options => options.UseInMemoryDatabase(_databaseName));

            using var provider = services.BuildServiceProvider();
            using var scope = provider.CreateScope();
            scope.ServiceProvider.GetRequiredService<SupportDeskDbContext>().Database.EnsureCreated();
        });
    }
}
