namespace VetPrescription.Api.Features.Vets.SaveVetProfile;

public static class SaveVetProfileEndpoint
{
    public static void Map(WebApplication app)
    {
        app.MapPost("/api/vets/profile", async (
            VetProfileRequest request,
            SaveVetProfileHandler handler,
            CancellationToken ct) => await handler.HandleAsync(request, ct))
            .WithName("saveVetProfile")
            .WithTags("Vets")
            .WithSummary("Save or update the veterinarian profile")
            .Produces<VetProfileResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
