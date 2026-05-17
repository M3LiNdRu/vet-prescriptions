using System.Net.Http.Json;
using VetPrescription.Api.Features.Prescriptions.Create;
using VetPrescription.Api.Features.Prescriptions.GeneratePdf;

namespace VetPrescription.IntegrationTests.Features.Prescriptions;

public class GetPrescriptionPdfUrlTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public GetPrescriptionPdfUrlTests(TestWebApplicationFactory factory)
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
    public async Task GetPrescriptionPdfUrl_WithValidIdAfterGenerate_Returns200WithPdfUrl()
    {
        var createRequest = ValidRequest();
        var createResponse = await _client.PostAsJsonAsync("/api/prescriptions", createRequest);
        var createdPrescription = await createResponse.Content.ReadFromJsonAsync<CreatePrescriptionResponse>();
        var prescriptionId = createdPrescription!.Id;

        // First generate the PDF
        await _client.PostAsJsonAsync<object>($"/api/prescriptions/{prescriptionId}/pdf", new object());

        // Then retrieve the PDF URL
        var urlResponse = await _client.GetAsync($"/api/prescriptions/{prescriptionId}/pdf");

        Assert.Equal(System.Net.HttpStatusCode.OK, urlResponse.StatusCode);
        var pdfData = await urlResponse.Content.ReadFromJsonAsync<PdfResponse>();
        Assert.NotNull(pdfData);
        Assert.NotNull(pdfData.Url);
        Assert.NotEmpty(pdfData.Url);
        Assert.Equal(createdPrescription.PrescriptionNumber, pdfData.PrescriptionNumber);
    }

    [Fact]
    public async Task GetPrescriptionPdfUrl_WithNonExistentId_Returns404()
    {
        var fakeId = "nonexistent-id-789";

        var response = await _client.GetAsync($"/api/prescriptions/{fakeId}/pdf");

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }
}
