namespace VetPrescription.Api.Features.Prescriptions.GetById;

public static class GetPrescriptionByIdEndpoint
{
    public static void Map(WebApplication app)
    {
        app.MapGet("/api/prescriptions/{id}", async (
            string id,
            GetPrescriptionByIdHandler handler,
            CancellationToken ct) => await handler.HandleAsync(id, ct))
            .WithName("getPrescriptionById")
            .WithTags("Prescriptions")
            .WithSummary("Retrieve a prescription by ID")
            .Produces<PrescriptionDetailResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
