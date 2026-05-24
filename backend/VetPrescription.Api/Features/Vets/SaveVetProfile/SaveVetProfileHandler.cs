using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;

namespace VetPrescription.Api.Features.Vets.SaveVetProfile;

public class SaveVetProfileHandler(
    ISaveVetProfileRepository repository,
    ILogger<SaveVetProfileHandler> logger)
{
    private static readonly SaveVetProfileValidator Validator = new();

    public async Task<Results<Ok<VetProfileResponse>, ValidationProblem>> HandleAsync(
        VetProfileRequest request,
        CancellationToken ct)
    {
        var validation = await Validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        await repository.UpsertAsync(request, ct);

        logger.LogInformation("Veterinary {VetName} saved profile", request.Name);

        return TypedResults.Ok(new VetProfileResponse(
            request.Name,
            request.LicenceNumber,
            request.ClinicName,
            request.Address,
            request.Phone,
            request.Email));
    }
}

public class SaveVetProfileValidator : AbstractValidator<VetProfileRequest>
{
    public SaveVetProfileValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.LicenceNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.ClinicName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Address).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}
