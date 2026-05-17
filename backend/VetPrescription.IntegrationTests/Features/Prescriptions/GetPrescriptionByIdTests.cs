using System.Net.Http.Json;
using VetPrescription.Api.Features.Prescriptions.Create;
using VetPrescription.Api.Features.Prescriptions.GetById;

namespace VetPrescription.IntegrationTests.Features.Prescriptions;

public class GetPrescriptionByIdTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public GetPrescriptionByIdTests(TestWebApplicationFactory factory)
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
    public async Task GetPrescriptionById_WithValidId_Returns200WithPrescriptionDetail()
    {
        var createRequest = ValidRequest();
        var createResponse = await _client.PostAsJsonAsync("/api/prescriptions", createRequest);
        var createdPrescription = await createResponse.Content.ReadFromJsonAsync<CreatePrescriptionResponse>();
        var prescriptionId = createdPrescription!.Id;

        var getResponse = await _client.GetAsync($"/api/prescriptions/{prescriptionId}");

        Assert.Equal(System.Net.HttpStatusCode.OK, getResponse.StatusCode);
        var detail = await getResponse.Content.ReadFromJsonAsync<PrescriptionDetailResponse>();
        Assert.NotNull(detail);
        Assert.Equal(prescriptionId, detail.Id);
        Assert.Equal("Dr. Joan", detail.Vet.Name);
        Assert.Equal("Owner", detail.Owner.Name);
        Assert.Equal("Rex", detail.Patient.AnimalName);
    }

    [Fact]
    public async Task GetPrescriptionById_WithNonExistentId_Returns404()
    {
        var fakeId = "nonexistent-id-123";

        var response = await _client.GetAsync($"/api/prescriptions/{fakeId}");

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }
}
