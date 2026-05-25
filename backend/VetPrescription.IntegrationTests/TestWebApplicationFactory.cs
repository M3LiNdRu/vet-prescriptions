using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Testcontainers.MongoDb;

namespace VetPrescription.IntegrationTests;

public class TestWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MongoDbContainer _mongo = new MongoDbBuilder("mongo:8.0")
        .Build();

    public async Task InitializeAsync()
    {
        await _mongo.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _mongo.DisposeAsync();
        await base.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:MongoDB"] = _mongo.GetConnectionString(),
                ["MongoDB:DatabaseName"] = "vet_prescription_test",
                ["Cors:AllowedOrigin"] = "http://localhost:5173",
                ["PdfsPath"] = Path.Combine(Path.GetTempPath(), "vet_pdfs_integration_test"),
                ["BaseUrl"] = "http://localhost",
            });
        });

        builder.UseEnvironment("Development");
    }
}
