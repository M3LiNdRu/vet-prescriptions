namespace VetPrescription.Domain.Entities;

public class PrescriptionItem
{
    public string DrugName { get; init; } = default!;
    public string Quantity { get; init; } = default!;
    public string PharmaceuticalForm { get; init; } = default!;
    public string DosageRegimen { get; init; } = default!;
    public string? WithdrawalPeriod { get; init; }
}
