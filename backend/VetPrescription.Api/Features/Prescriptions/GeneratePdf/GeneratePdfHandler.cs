using Microsoft.AspNetCore.Http.HttpResults;
using VetPrescription.Api.Infrastructure;

namespace VetPrescription.Api.Features.Prescriptions.GeneratePdf;

public class GeneratePdfHandler(
    IGeneratePdfRepository repository,
    IConfiguration configuration,
    ILogger<GeneratePdfHandler> logger)
{
    public async Task<Results<Created<PdfResponse>, NotFound>> HandleAsync(
        string id,
        CancellationToken ct)
    {
        var prescription = await repository.GetByIdAsync(id, ct);
        if (prescription is null)
            return TypedResults.NotFound();

        var pdfBytes = PdfGenerator.Generate(new PrescriptionDocument(
            prescription.PrescriptionNumber,
            prescription.Date,
            prescription.Vet.Name,
            prescription.Vet.LicenceNumber,
            prescription.Vet.ClinicName,
            prescription.Vet.Address,
            prescription.Vet.Phone,
            prescription.Vet.Email,
            prescription.Owner.Name,
            prescription.Owner.Address,
            prescription.Owner.Phone,
            prescription.Owner.CifDni,
            prescription.Patient.AnimalName,
            prescription.Patient.Species,
            prescription.Patient.Breed,
            prescription.Items.Select(i => new PrescriptionItemRow(i.DrugName, i.Quantity, i.PharmaceuticalForm, i.DosageRegimen, i.WithdrawalPeriod)),
            prescription.Warnings,
            prescription.IsOffLabel,
            prescription.IsAntimicrobialSpecialUse));

        var pdfsPath = configuration["PdfsPath"] ?? "/app/pdfs";
        Directory.CreateDirectory(pdfsPath);
        var fileName = $"{prescription.PrescriptionNumber}.pdf";
        await File.WriteAllBytesAsync(Path.Combine(pdfsPath, fileName), pdfBytes, ct);

        var baseUrl = configuration["BaseUrl"] ?? "http://localhost:8080";
        var pdfUrl = $"{baseUrl}/pdfs/{fileName}";

        await repository.UpdatePdfUrlAsync(id, pdfUrl, ct);

        logger.LogInformation(
            "Veterinary {VetName} generated PDF for prescription {PrescriptionNumber}",
            prescription.Vet.Name,
            prescription.PrescriptionNumber);

        return TypedResults.Created(pdfUrl, new PdfResponse(pdfUrl, prescription.PrescriptionNumber));
    }
}

public record PdfResponse(string Url, string PrescriptionNumber);
