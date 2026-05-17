namespace VetPrescription.Domain.Entities;

public class Patient
{
    public string AnimalName { get; init; } = default!;
    public string Species { get; init; } = default!;
    public string Breed { get; init; } = default!;
}
