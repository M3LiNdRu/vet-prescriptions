using Microsoft.AspNetCore.Http.HttpResults;
using VetPrescription.Api.Features.Vets.SaveVetProfile;

namespace VetPrescription.Api.Features.Vets.GetVetProfile;

public class GetVetProfileHandler(
    IGetVetProfileRepository repository,
    ILogger<GetVetProfileHandler> logger)
{
    public async Task<Results<Ok<VetProfileResponse>, NotFound>> HandleAsync(CancellationToken ct)
    {
        var profile = await repository.GetAsync(ct);
        if (profile is null)
            return TypedResults.NotFound();

        logger.LogInformation("Veterinary retrieved profile");

        return TypedResults.Ok(profile);
    }
}
