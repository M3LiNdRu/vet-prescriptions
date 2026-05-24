namespace VetPrescription.Api.Features.Vets.SaveVetProfile;

public record VetProfileRequest(
    string Name,
    string LicenceNumber,
    string ClinicName,
    string Address,
    string Phone,
    string Email);

public record VetProfileResponse(
    string Name,
    string LicenceNumber,
    string ClinicName,
    string Address,
    string Phone,
    string Email);
