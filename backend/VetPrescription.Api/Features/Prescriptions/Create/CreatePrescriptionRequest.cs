namespace VetPrescription.Api.Features.Prescriptions.Create;

public record CreatePrescriptionRequest(
    VetRequest Vet,
    OwnerRequest Owner,
    PatientRequest Patient,
    List<PrescriptionItemRequest> Items,
    string? Warnings,
    bool IsOffLabel = false,
    bool IsAntimicrobialSpecialUse = false);

public record VetRequest(
    string Name,
    string LicenceNumber,
    string ClinicName,
    string Address,
    string Phone,
    string Email);

public record OwnerRequest(
    string Name,
    string Address,
    string Phone,
    string CifDni);

public record PatientRequest(
    string AnimalName,
    string Species,
    string Breed);

public record PrescriptionItemRequest(
    string DrugName,
    string Quantity,
    string PharmaceuticalForm,
    string DosageRegimen,
    string? WithdrawalPeriod);
