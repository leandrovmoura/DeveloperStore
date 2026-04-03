using Ambev.DeveloperEvaluation.ORM;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.Functional;

/// <summary>
/// Custom WebApplicationFactory for functional tests
/// </summary>
public class WebApplicationFactory<TProgram> : Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory<TProgram> where TProgram : class
{
    private static readonly string DatabaseName = $"InMemoryDbForTesting_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<DefaultContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add DbContext using an in-memory database for testing
            services.AddDbContext<DefaultContext>(options =>
            {
                options.UseInMemoryDatabase(DatabaseName);
            });

            // Build the service provider
            var sp = services.BuildServiceProvider();

            // Create a scope to obtain a reference to the database context
            using (var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<DefaultContext>();

                // Ensure the database is created
                db.Database.EnsureCreated();
            }
        });
    }
}