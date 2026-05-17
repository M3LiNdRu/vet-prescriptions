using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using Moq;
using VetPrescription.Api.Features.Prescriptions.GeneratePdf;
using VetPrescription.Api.Features.Prescriptions.GetPrescriptionPdfUrl;

namespace VetPrescription.UnitTests.Features.Prescriptions;

public class GetPrescriptionPdfUrlHandlerTests
{
    private readonly Mock<IGetPrescriptionPdfUrlRepository> _repo = new();
    private readonly Mock<ILogger<GetPrescriptionPdfUrlHandler>> _logger = new();
    private GetPrescriptionPdfUrlHandler Sut() => new(_repo.Object, _logger.Object);

    [Fact]
    public async Task HandleAsync_PdfExists_Returns200WithUrl()
    {
        _repo.Setup(r => r.GetPdfInfoAsync("id1", It.IsAny<CancellationToken>()))
             .ReturnsAsync(("RX-2026-0001", "http://localhost:8080/pdfs/RX-2026-0001.pdf"));

        var result = await Sut().HandleAsync("id1", CancellationToken.None);

        var ok = Assert.IsType<Ok<PdfResponse>>(result.Result);
        Assert.Equal("http://localhost:8080/pdfs/RX-2026-0001.pdf", ok.Value!.Url);
        Assert.Equal("RX-2026-0001", ok.Value.PrescriptionNumber);
    }

    [Fact]
    public async Task HandleAsync_PrescriptionNotFound_Returns404()
    {
        _repo.Setup(r => r.GetPdfInfoAsync("bad", It.IsAny<CancellationToken>()))
             .ReturnsAsync(((string?, string?)?)null);

        var result = await Sut().HandleAsync("bad", CancellationToken.None);

        Assert.IsType<NotFound>(result.Result);
    }

    [Fact]
    public async Task HandleAsync_PdfNotYetGenerated_Returns404()
    {
        _repo.Setup(r => r.GetPdfInfoAsync("id1", It.IsAny<CancellationToken>()))
             .ReturnsAsync(("RX-2026-0001", null));

        var result = await Sut().HandleAsync("id1", CancellationToken.None);

        Assert.IsType<NotFound>(result.Result);
    }

    [Fact]
    public async Task HandleAsync_PdfExists_LogsAuditMessage()
    {
        _repo.Setup(r => r.GetPdfInfoAsync("id1", It.IsAny<CancellationToken>()))
             .ReturnsAsync(("RX-2026-0001", "http://localhost:8080/pdfs/RX-2026-0001.pdf"));

        await Sut().HandleAsync("id1", CancellationToken.None);

        _logger.Verify(l => l.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("retrieved PDF URL")),
            null,
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }
}
