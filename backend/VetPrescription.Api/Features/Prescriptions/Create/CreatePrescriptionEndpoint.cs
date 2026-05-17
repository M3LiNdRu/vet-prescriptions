namespace VetPrescription.Api.Features.Prescriptions.Create;

public static class CreatePrescriptionEndpoint
{
    public static void Map(WebApplication app)
    {
        app.MapPost("/api/prescriptions", async (
            CreatePrescriptionRequest request,
            CreatePrescriptionHandler handler,
            CancellationToken ct) => await handler.HandleAsync(request, ct))
            .WithName("createPrescription")
            .WithTags("Prescriptions")
            .WithSummary("Create a new veterinary prescription")
            .Produces<CreatePrescriptionResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
