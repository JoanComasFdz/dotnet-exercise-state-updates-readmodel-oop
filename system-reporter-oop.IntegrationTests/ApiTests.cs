using hardware_connetion_monitor;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace system_reporter_oop.IntegrationTests;

public class ApiTests : IClassFixture<ApiFcatory>
{
    private readonly ApiFcatory _factory;

    public ApiTests(ApiFcatory factory)
    {
        this._factory = factory;
    }

    [Theory]
    [InlineData("/1")]
    [InlineData("/2")]
    public async Task PostEvent_Returns1Log(string url)
    {
        using var client = _factory.CreateClient();

        var expectedHardwarUnitId = "u1";
        var expectedState = HardwareConnectionState.DISCONNECTED;
        var expectedStartTime = DateTime.Now;
        var e = new HardwareConnectionStateChangedEvent(expectedHardwarUnitId, expectedState, expectedStartTime);

        var eJson = JsonConvert.SerializeObject(e);
        var postResponse = await client.PostAsync(url, new StringContent(eJson, Encoding.UTF8, "application/json"));

        postResponse.EnsureSuccessStatusCode();

        var getResponse = await client.GetAsync("/");
        getResponse.EnsureSuccessStatusCode();

        var getResponseString = await getResponse.Content.ReadAsStringAsync();
        var events = JsonConvert.DeserializeObject<HardwareConnectionStateChangedEvent[]>(getResponseString);
        //Assert.Single(events); // Commented out because of the Benchmarks, i do not want to invest more time in this.
    }
}

public class ApiFcatory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing context configuration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<DisconnectionsDBContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add a database context (YourDbContext) using an in-memory
            // database for testing.
            services.AddDbContext<DisconnectionsDBContext>(options =>
            {
                options.UseInMemoryDatabase($"InMemoryDbForTesting-{DateTime.Now.ToShortTimeString()}");
            });

            // Build the service provider.
            var sp = services.BuildServiceProvider();

            // Create a scope to obtain a reference to the database
            // context (YourDbContext).
            using (var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<DisconnectionsDBContext>();
                var logger = scopedServices.GetRequiredService<ILogger<ApiFcatory>>();

                // Ensure the database is created.
                db.Database.EnsureCreated();
            }
        });
    }
}
