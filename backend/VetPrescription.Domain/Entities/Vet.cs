namespace VetPrescription.Domain.Entities;

public class Vet
{
    public string Name { get; init; } = default!;
    public string LicenceNumber { get; init; } = default!;
    public string ClinicName { get; init; } = default!;
    public string Address { get; init; } = default!;
    public string Phone { get; init; } = default!;
    public string Email { get; init; } = default!;
}
