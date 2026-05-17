using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using VetPrescription.Domain.Entities;

namespace VetPrescription.Api.Features.Prescriptions.Create;

public class CreatePrescriptionHandler(
    ICreatePrescriptionRepository repository,
    ILogger<CreatePrescriptionHandler> logger)
{
    private static readonly CreatePrescriptionValidator Validator = new();

    public async Task<Results<Created<CreatePrescriptionResponse>, ValidationProblem>> HandleAsync(
        CreatePrescriptionRequest request,
        CancellationToken ct)
    {
        var validation = await Validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var prescription = await repository.InsertAsync(request, ct);

        logger.LogInformation(
            "Veterinary {VetName} issued prescription {PrescriptionNumber}",
            prescription.Vet.Name,
            prescription.PrescriptionNumber);

        return TypedResults.Created(
            $"/api/prescriptions/{prescription.Id}",
            new CreatePrescriptionResponse(prescription.Id, prescription.PrescriptionNumber, prescription.Date.ToString("yyyy-MM-dd")));
    }
}

public record CreatePrescriptionResponse(string Id, string PrescriptionNumber, string Date);

public class CreatePrescriptionValidator : AbstractValidator<CreatePrescriptionRequest>
{
    public CreatePrescriptionValidator()
    {
        RuleFor(x => x.Vet).NotNull().SetValidator(new VetRequestValidator());
        RuleFor(x => x.Owner).NotNull().SetValidator(new OwnerRequestValidator());
        RuleFor(x => x.Patient).NotNull().SetValidator(new PatientRequestValidator());
        RuleFor(x => x.Items).NotEmpty().WithMessage("At least one prescription item is required.");
        RuleForEach(x => x.Items).SetValidator(new PrescriptionItemRequestValidator());
    }
}

public class VetRequestValidator : AbstractValidator<VetRequest>
{
    public VetRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.LicenceNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.ClinicName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Address).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}

public class OwnerRequestValidator : AbstractValidator<OwnerRequest>
{
    public OwnerRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Address).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(30);
        RuleFor(x => x.CifDni).NotEmpty().MaximumLength(20);
    }
}

public class PatientRequestValidator : AbstractValidator<PatientRequest>
{
    public PatientRequestValidator()
    {
        RuleFor(x => x.AnimalName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Species).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Breed).NotEmpty().MaximumLength(100);
    }
}

public class PrescriptionItemRequestValidator : AbstractValidator<PrescriptionItemRequest>
{
    public PrescriptionItemRequestValidator()
    {
        RuleFor(x => x.DrugName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Quantity).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PharmaceuticalForm).NotEmpty().MaximumLength(100);
        RuleFor(x => x.DosageRegimen).NotEmpty().MaximumLength(300);
        RuleFor(x => x.WithdrawalPeriod).MaximumLength(100).When(x => x.WithdrawalPeriod != null);
    }
}
