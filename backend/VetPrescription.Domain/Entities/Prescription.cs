namespace VetPrescription.Domain.Entities;

public class Prescription
{
    public string Id { get; init; } = default!;
    public string PrescriptionNumber { get; init; } = default!;
    public DateOnly Date { get; init; }
    public Vet Vet { get; init; } = default!;
    public Owner Owner { get; init; } = default!;
    public Patient Patient { get; init; } = default!;
    public IReadOnlyList<PrescriptionItem> Items { get; init; } = [];
    public string? Warnings { get; init; }
    public bool IsOffLabel { get; init; }
    public bool IsAntimicrobialSpecialUse { get; init; }
    public string? PdfUrl { get; init; }

    // Called by the Repository once it has the next sequence number from MongoDB
    public static Prescription Create(
        string id,
        int sequenceNumber,
        Vet vet,
        Owner owner,
        Patient patient,
        IReadOnlyList<PrescriptionItem> items,
        string? warnings,
        bool isOffLabel,
        bool isAntimicrobialSpecialUse)
    {
        var year = DateTime.UtcNow.Year;
        return new Prescription
        {
            Id = id,
            PrescriptionNumber = $"RX-{year}-{sequenceNumber:D4}",
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            Vet = vet,
            Owner = owner,
            Patient = patient,
            Items = items,
            Warnings = warnings,
            IsOffLabel = isOffLabel,
            IsAntimicrobialSpecialUse = isAntimicrobialSpecialUse,
        };
    }
}
