using Microsoft.AspNetCore.Http.HttpResults;
using VetPrescription.Api.Features.Prescriptions.GeneratePdf;

namespace VetPrescription.Api.Features.Prescriptions.GetPrescriptionPdfUrl;

public class GetPrescriptionPdfUrlHandler(
    IGetPrescriptionPdfUrlRepository repository,
    ILogger<GetPrescriptionPdfUrlHandler> logger)
{
    public async Task<Results<Ok<PdfResponse>, NotFound>> HandleAsync(
        string id,
        CancellationToken ct)
    {
        var info = await repository.GetPdfInfoAsync(id, ct);

        if (info is null)
            return TypedResults.NotFound();

        var (prescriptionNumber, pdfUrl) = info.Value;

        if (string.IsNullOrEmpty(pdfUrl))
            return TypedResults.NotFound();

        logger.LogInformation(
            "Veterinary retrieved PDF URL for prescription {PrescriptionNumber}",
            prescriptionNumber);

        return TypedResults.Ok(new PdfResponse(pdfUrl, prescriptionNumber!));
    }
}
