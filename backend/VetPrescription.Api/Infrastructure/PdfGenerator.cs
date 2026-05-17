using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace VetPrescription.Api.Infrastructure;

public static class PdfGenerator
{
    public static byte[] Generate(PrescriptionDocument document)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.Content().Element(document.Compose);
            });
        }).GeneratePdf();
    }
}

public record PrescriptionDocument(
    string PrescriptionNumber,
    DateOnly Date,
    string VetName,
    string VetLicenceNumber,
    string VetClinicName,
    string VetAddress,
    string VetPhone,
    string VetEmail,
    string OwnerName,
    string OwnerAddress,
    string OwnerPhone,
    string OwnerCifDni,
    string AnimalName,
    string Species,
    string Breed,
    IEnumerable<PrescriptionItemRow> Items,
    string? Warnings,
    bool IsOffLabel,
    bool IsAntimicrobialSpecialUse)
{
    public void Compose(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text(VetClinicName).FontSize(14).Bold();
                    c.Item().Text(VetName).FontSize(11);
                    c.Item().Text($"Col. {VetLicenceNumber}").FontSize(10);
                    c.Item().Text(VetAddress).FontSize(10);
                    c.Item().Text(VetPhone).FontSize(10);
                    c.Item().Text(VetEmail).FontSize(10);
                });
                row.ConstantItem(150).AlignRight().Column(c =>
                {
                    c.Item().Text("RECEPTA VETERINÀRIA").FontSize(12).Bold();
                    c.Item().Text($"Núm: {PrescriptionNumber}").FontSize(10);
                    c.Item().Text($"Data: {Date:dd/MM/yyyy}").FontSize(10);
                });
            });

            col.Item().PaddingTop(10).LineHorizontal(1);

            col.Item().PaddingTop(8).Column(c =>
            {
                c.Item().Text("PROPIETARI").FontSize(9).Bold();
                c.Item().Text($"{OwnerName} — {OwnerCifDni}").FontSize(10);
                c.Item().Text($"{OwnerAddress} · {OwnerPhone}").FontSize(10);
            });

            col.Item().PaddingTop(8).Column(c =>
            {
                c.Item().Text("ANIMAL").FontSize(9).Bold();
                c.Item().Text($"{AnimalName} · {Species} · {Breed}").FontSize(10);
            });

            col.Item().PaddingTop(10).LineHorizontal(1);

            col.Item().PaddingTop(8).Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.RelativeColumn(3);
                    cols.RelativeColumn(1);
                    cols.RelativeColumn(2);
                    cols.RelativeColumn(3);
                    cols.RelativeColumn(2);
                });

                table.Header(h =>
                {
                    foreach (var label in new[] { "Medicament", "Quantitat", "Forma farm.", "Posologia", "Temps espera" })
                        h.Cell().Text(label).FontSize(9).Bold();
                });

                foreach (var item in Items)
                {
                    table.Cell().Text(item.DrugName).FontSize(10);
                    table.Cell().Text(item.Quantity).FontSize(10);
                    table.Cell().Text(item.PharmaceuticalForm).FontSize(10);
                    table.Cell().Text(item.DosageRegimen).FontSize(10);
                    table.Cell().Text(item.WithdrawalPeriod ?? "—").FontSize(10);
                }
            });

            if (!string.IsNullOrWhiteSpace(Warnings))
            {
                col.Item().PaddingTop(8).Column(c =>
                {
                    c.Item().Text("Advertències:").FontSize(9).Bold();
                    c.Item().Text(Warnings).FontSize(10);
                });
            }

            if (IsOffLabel)
                col.Item().PaddingTop(4).Text("* Prescripció al marge de l'autorització de comercialització (Art. 112/113/114 Regl. UE 2019/6)").FontSize(8).Italic();

            if (IsAntimicrobialSpecialUse)
                col.Item().PaddingTop(2).Text("* Ús especial d'antimicrobians (Art. 107.3/4 Regl. UE 2019/6)").FontSize(8).Italic();

            col.Item().PaddingTop(20).Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("Signatura del veterinari:").FontSize(9).Bold();
                    c.Item().PaddingTop(30).LineHorizontal(1);
                    c.Item().PaddingTop(2).Text(VetName).FontSize(9);
                });
            });
        });
    }
}

public record PrescriptionItemRow(
    string DrugName,
    string Quantity,
    string PharmaceuticalForm,
    string DosageRegimen,
    string? WithdrawalPeriod);
