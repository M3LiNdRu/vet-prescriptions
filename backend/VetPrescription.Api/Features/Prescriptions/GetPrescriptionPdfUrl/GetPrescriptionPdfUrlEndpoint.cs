using VetPrescription.Api.Features.Prescriptions.GeneratePdf;

namespace VetPrescription.Api.Features.Prescriptions.GetPrescriptionPdfUrl;

public static class GetPrescriptionPdfUrlEndpoint
{
    public static void Map(WebApplication app)
    {
        app.MapGet("/api/prescriptions/{id}/pdf", async (
            string id,
            GetPrescriptionPdfUrlHandler handler,
            CancellationToken ct) => await handler.HandleAsync(id, ct))
            .WithName("getPrescriptionPdfUrl")
            .WithTags("Prescriptions")
            .WithSummary("Retrieve the URL of the stored PDF for a prescription")
            .Produces<PdfResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
