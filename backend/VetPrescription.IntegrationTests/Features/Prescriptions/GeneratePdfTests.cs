using System.Net.Http.Json;
using VetPrescription.Api.Features.Prescriptions.Create;
using VetPrescription.Api.Features.Prescriptions.GeneratePdf;

namespace VetPrescription.IntegrationTests.Features.Prescriptions;

[Collection(IntegrationTestCollection.Name)]
public class GeneratePdfTests
{
    private readonly HttpClient _client;

    public GeneratePdfTests(TestWebApplicationFactory factory)
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
    public async Task GeneratePdf_WithValidPrescriptionId_Returns201WithPdfUrl()
    {
        var createRequest = ValidRequest();
        var createResponse = await _client.PostAsJsonAsync("/api/prescriptions", createRequest);
        var createdPrescription = await createResponse.Content.ReadFromJsonAsync<CreatePrescriptionResponse>();
        var prescriptionId = createdPrescription!.Id;

        var pdfResponse = await _client.PostAsJsonAsync<object>($"/api/prescriptions/{prescriptionId}/pdf", new object());

        Assert.Equal(System.Net.HttpStatusCode.Created, pdfResponse.StatusCode);
        var pdfData = await pdfResponse.Content.ReadFromJsonAsync<PdfResponse>();
        Assert.NotNull(pdfData);
        Assert.NotNull(pdfData.Url);
        Assert.NotEmpty(pdfData.Url);
        Assert.Equal(createdPrescription.PrescriptionNumber, pdfData.PrescriptionNumber);
    }

    [Fact]
    public async Task GeneratePdf_WithNonExistentId_Returns404()
    {
        var fakeId = "nonexistent-id-456";

        var response = await _client.PostAsJsonAsync<object>($"/api/prescriptions/{fakeId}/pdf", new object());

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }
}
