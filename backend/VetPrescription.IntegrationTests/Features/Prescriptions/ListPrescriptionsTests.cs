using System.Net.Http.Json;
using VetPrescription.Api.Features.Prescriptions.Create;
using VetPrescription.Api.Features.Prescriptions.List;

namespace VetPrescription.IntegrationTests.Features.Prescriptions;

[Collection(IntegrationTestCollection.Name)]
public class ListPrescriptionsTests
{
    private readonly HttpClient _client;

    public ListPrescriptionsTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private static CreatePrescriptionRequest ValidRequest(string animalName) => new(
        new VetRequest("Dr. Joan", "CAT-1", "Clinic", "Addr", "+34600", "j@j.cat"),
        new OwnerRequest("Owner", "Addr", "+34600", "12345678A"),
        new PatientRequest(animalName, "Canis", "Labrador"),
        new List<PrescriptionItemRequest>
        {
            new("DrugA", "1 box", "Tablets", "1/day", null)
        },
        null, false, false);

    [Fact]
    public async Task ListPrescriptions_Returns200WithArray()
    {
        var response = await _client.GetAsync("/api/prescriptions");

        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        var items = await response.Content.ReadFromJsonAsync<List<PrescriptionSummaryResponse>>();
        Assert.NotNull(items);
    }

    [Fact]
    public async Task ListPrescriptions_AfterCreating_ContainsNewPrescription()
    {
        await _client.PostAsJsonAsync("/api/prescriptions", ValidRequest("TestAnimal"));

        var response = await _client.GetAsync("/api/prescriptions");
        var items = await response.Content.ReadFromJsonAsync<List<PrescriptionSummaryResponse>>();

        Assert.NotNull(items);
        Assert.Contains(items, p => p.PatientName == "TestAnimal");
    }
}
