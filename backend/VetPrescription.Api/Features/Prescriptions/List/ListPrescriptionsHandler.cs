using Microsoft.AspNetCore.Http.HttpResults;

namespace VetPrescription.Api.Features.Prescriptions.List;

public class ListPrescriptionsHandler(
    IListPrescriptionsRepository repository,
    ILogger<ListPrescriptionsHandler> logger)
{
    public async Task<Ok<IReadOnlyList<PrescriptionSummaryResponse>>> HandleAsync(CancellationToken ct)
    {
        var prescriptions = await repository.GetAllAsync(ct);

        logger.LogInformation("Veterinary listed {Count} prescriptions", prescriptions.Count);

        return TypedResults.Ok(prescriptions);
    }
}

public record PrescriptionSummaryResponse(
    string Id,
    string PrescriptionNumber,
    string Date,
    string PatientName,
    string VetName);
