using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace VetPrescription.IntegrationTests;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:MongoDB"] = "mongodb://localhost:27017",
                ["MongoDB:DatabaseName"] = "vet_prescription_test",
                ["Cors:AllowedOrigin"] = "http://localhost:5173",
            });
        });

        builder.UseEnvironment("Development");
    }
}
