namespace VetPrescription.Api.Features.Prescriptions.GeneratePdf;

public static class GeneratePdfEndpoint
{
    public static void Map(WebApplication app)
    {
        app.MapPost("/api/prescriptions/{id}/pdf", async (
            string id,
            GeneratePdfHandler handler,
            CancellationToken ct) => await handler.HandleAsync(id, ct))
            .WithName("generatePrescriptionPdf")
            .WithTags("Prescriptions")
            .WithSummary("Generate and store the PDF for a prescription")
            .Produces<PdfResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
