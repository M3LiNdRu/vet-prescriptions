namespace VetPrescription.Api.Features.Prescriptions.List;

public static class ListPrescriptionsEndpoint
{
    public static void Map(WebApplication app)
    {
        app.MapGet("/api/prescriptions", async (
            ListPrescriptionsHandler handler,
            CancellationToken ct) => await handler.HandleAsync(ct))
            .WithName("listPrescriptions")
            .WithTags("Prescriptions")
            .WithSummary("List all prescriptions sorted by date descending")
            .Produces<IReadOnlyList<PrescriptionSummaryResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
