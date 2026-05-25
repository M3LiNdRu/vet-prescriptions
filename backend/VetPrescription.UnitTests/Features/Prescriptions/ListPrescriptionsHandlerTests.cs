using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using Moq;
using VetPrescription.Api.Features.Prescriptions.List;

namespace VetPrescription.UnitTests.Features.Prescriptions;

public class ListPrescriptionsHandlerTests
{
    private readonly Mock<IListPrescriptionsRepository> _repo = new();
    private readonly Mock<ILogger<ListPrescriptionsHandler>> _logger = new();
    private ListPrescriptionsHandler Sut() => new(_repo.Object, _logger.Object);

    private static IReadOnlyList<PrescriptionSummaryResponse> SampleList() =>
    [
        new("id1", "RX-2026-0002", "2026-05-17", "Rex", "Dr. Joan"),
        new("id2", "RX-2026-0001", "2026-05-16", "Luna", "Dr. Joan"),
    ];

    [Fact]
    public async Task HandleAsync_ReturnsList()
    {
        _repo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
             .ReturnsAsync(SampleList());

        var result = await Sut().HandleAsync(CancellationToken.None);

        var ok = Assert.IsType<Ok<IReadOnlyList<PrescriptionSummaryResponse>>>(result);
        Assert.Equal(2, ok.Value!.Count);
        Assert.Equal("RX-2026-0002", ok.Value[0].PrescriptionNumber);
    }

    [Fact]
    public async Task HandleAsync_EmptyList_ReturnsEmptyArray()
    {
        _repo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
             .ReturnsAsync([]);

        var result = await Sut().HandleAsync(CancellationToken.None);

        var ok = Assert.IsType<Ok<IReadOnlyList<PrescriptionSummaryResponse>>>(result);
        Assert.Empty(ok.Value!);
    }

    [Fact]
    public async Task HandleAsync_LogsAuditMessage()
    {
        _repo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
             .ReturnsAsync(SampleList());

        await Sut().HandleAsync(CancellationToken.None);

        _logger.Verify(l => l.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("listed")),
            null,
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }
}
