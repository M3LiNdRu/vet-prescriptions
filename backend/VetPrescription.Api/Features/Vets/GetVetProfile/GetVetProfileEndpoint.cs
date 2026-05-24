using VetPrescription.Api.Features.Vets.SaveVetProfile;

namespace VetPrescription.Api.Features.Vets.GetVetProfile;

public static class GetVetProfileEndpoint
{
    public static void Map(WebApplication app)
    {
        app.MapGet("/api/vets/profile", async (
            GetVetProfileHandler handler,
            CancellationToken ct) => await handler.HandleAsync(ct))
            .WithName("getVetProfile")
            .WithTags("Vets")
            .WithSummary("Retrieve the saved veterinarian profile")
            .Produces<VetProfileResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
