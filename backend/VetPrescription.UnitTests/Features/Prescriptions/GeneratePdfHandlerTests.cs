using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using VetPrescription.Api.Features.Prescriptions.GeneratePdf;
using VetPrescription.Domain.Entities;

namespace VetPrescription.UnitTests.Features.Prescriptions;

public class GeneratePdfHandlerTests
{
    private readonly Mock<IGeneratePdfRepository> _repo = new();
    private readonly Mock<ILogger<GeneratePdfHandler>> _logger = new();
    private readonly IConfiguration _config = new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["PdfsPath"] = Path.Combine(Path.GetTempPath(), "vet_pdfs_test"),
            ["BaseUrl"] = "http://localhost:8080",
        }).Build();

    private GeneratePdfHandler Sut() => new(_repo.Object, _config, _logger.Object);

    private static Prescription SamplePrescription() => Prescription.Create(
        "id1", 1,
        new Vet { Name = "Dr. Joan", LicenceNumber = "CAT-1", ClinicName = "Clinic", Address = "Addr", Phone = "+34600", Email = "j@j.cat" },
        new Owner { Name = "Owner", Address = "Addr", Phone = "+34600", CifDni = "12345678A" },
        new Patient { AnimalName = "Rex", Species = "Canis", Breed = "Labrador" },
        [new PrescriptionItem { DrugName = "DrugA", Quantity = "1 box", PharmaceuticalForm = "Tablets", DosageRegimen = "1/day" }],
        null, false, false);

    [Fact]
    public async Task HandleAsync_ExistingPrescription_Returns201WithUrl()
    {
        _repo.Setup(r => r.GetByIdAsync("id1", It.IsAny<CancellationToken>()))
             .ReturnsAsync(SamplePrescription());
        _repo.Setup(r => r.UpdatePdfUrlAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
             .Returns(Task.CompletedTask);

        var result = await Sut().HandleAsync("id1", CancellationToken.None);

        var created = Assert.IsType<Created<PdfResponse>>(result.Result);
        Assert.Equal(201, created.StatusCode);
        Assert.Contains(".pdf", created.Value!.Url);
    }

    [Fact]
    public async Task HandleAsync_UnknownId_Returns404()
    {
        _repo.Setup(r => r.GetByIdAsync("bad", It.IsAny<CancellationToken>()))
             .ReturnsAsync((Prescription?)null);

        var result = await Sut().HandleAsync("bad", CancellationToken.None);

        Assert.IsType<NotFound>(result.Result);
    }

    [Fact]
    public async Task HandleAsync_Success_LogsAuditMessage()
    {
        _repo.Setup(r => r.GetByIdAsync("id1", It.IsAny<CancellationToken>()))
             .ReturnsAsync(SamplePrescription());
        _repo.Setup(r => r.UpdatePdfUrlAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
             .Returns(Task.CompletedTask);

        await Sut().HandleAsync("id1", CancellationToken.None);

        _logger.Verify(l => l.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("generated PDF")),
            null,
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }
}
