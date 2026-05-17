using Microsoft.AspNetCore.Http.HttpResults;

namespace VetPrescription.Api.Features.Prescriptions.GetById;

public class GetPrescriptionByIdHandler(
    IGetPrescriptionByIdRepository repository,
    ILogger<GetPrescriptionByIdHandler> logger)
{
    public async Task<Results<Ok<PrescriptionDetailResponse>, NotFound>> HandleAsync(
        string id,
        CancellationToken ct)
    {
        var prescription = await repository.GetByIdAsync(id, ct);
        if (prescription is null)
            return TypedResults.NotFound();

        logger.LogInformation(
            "Veterinary retrieved prescription {PrescriptionNumber}",
            prescription.PrescriptionNumber);

        return TypedResults.Ok(new PrescriptionDetailResponse(
            prescription.Id,
            prescription.PrescriptionNumber,
            prescription.Date.ToString("yyyy-MM-dd"),
            new VetDetailDto(prescription.Vet.Name, prescription.Vet.LicenceNumber, prescription.Vet.ClinicName, prescription.Vet.Address, prescription.Vet.Phone, prescription.Vet.Email),
            new OwnerDetailDto(prescription.Owner.Name, prescription.Owner.Address, prescription.Owner.Phone, prescription.Owner.CifDni),
            new PatientDetailDto(prescription.Patient.AnimalName, prescription.Patient.Species, prescription.Patient.Breed),
            prescription.Items.Select(i => new PrescriptionItemDetailDto(i.DrugName, i.Quantity, i.PharmaceuticalForm, i.DosageRegimen, i.WithdrawalPeriod)).ToList(),
            prescription.Warnings,
            prescription.IsOffLabel,
            prescription.IsAntimicrobialSpecialUse));
    }
}

public record PrescriptionDetailResponse(
    string Id,
    string PrescriptionNumber,
    string Date,
    VetDetailDto Vet,
    OwnerDetailDto Owner,
    PatientDetailDto Patient,
    List<PrescriptionItemDetailDto> Items,
    string? Warnings,
    bool IsOffLabel,
    bool IsAntimicrobialSpecialUse);

public record VetDetailDto(string Name, string LicenceNumber, string ClinicName, string Address, string Phone, string Email);
public record OwnerDetailDto(string Name, string Address, string Phone, string CifDni);
public record PatientDetailDto(string AnimalName, string Species, string Breed);
public record PrescriptionItemDetailDto(string DrugName, string Quantity, string PharmaceuticalForm, string DosageRegimen, string? WithdrawalPeriod);
