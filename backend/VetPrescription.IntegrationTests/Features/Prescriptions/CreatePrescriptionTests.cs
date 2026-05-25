using System.Net.Http.Json;
using VetPrescription.Api.Features.Prescriptions.Create;

namespace VetPrescription.IntegrationTests.Features.Prescriptions;

[Collection(IntegrationTestCollection.Name)]
public class CreatePrescriptionTests
{
    private readonly HttpClient _client;

    public CreatePrescriptionTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private static CreatePrescriptionRequest ValidRequest() => new(
        new VetRequest("Dr. Joan", "CAT-1", "Clinic", "Addr", "+34600", "j@j.cat"),
        new OwnerRequest("Owner", "Addr", "+34600", "12345678A"),
        new PatientRequest("Rex", "Canis", "Labrador"),
        new List<PrescriptionItemRequest>
        {
            new PrescriptionItemRequest("DrugA", "1 box", "Tablets", "1/day for 7d", null)
        },
        null, false, false);

    [Fact]
    public async Task CreatePrescription_WithValidRequest_Returns201WithIdAndNumber()
    {
        var request = ValidRequest();

        var response = await _client.PostAsJsonAsync("/api/prescriptions", request);

        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<CreatePrescriptionResponse>();
        Assert.NotNull(content);
        Assert.NotNull(content.Id);
        Assert.StartsWith("RX-", content.PrescriptionNumber);
        Assert.NotNull(content.Date);
    }

    [Fact]
    public async Task CreatePrescription_WithMissingVetName_Returns400()
    {
        var request = ValidRequest() with
        {
            Vet = new VetRequest("", "CAT-1", "Clinic", "Addr", "+34600", "j@j.cat")
        };

        var response = await _client.PostAsJsonAsync("/api/prescriptions", request);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreatePrescription_WithEmptyItems_Returns400()
    {
        var request = ValidRequest() with { Items = new List<PrescriptionItemRequest>() };

        var response = await _client.PostAsJsonAsync("/api/prescriptions", request);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }
}
