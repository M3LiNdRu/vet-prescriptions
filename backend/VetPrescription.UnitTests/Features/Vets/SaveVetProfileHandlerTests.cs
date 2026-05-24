using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using Moq;
using VetPrescription.Api.Features.Vets.SaveVetProfile;

namespace VetPrescription.UnitTests.Features.Vets;

public class SaveVetProfileHandlerTests
{
    private readonly Mock<ISaveVetProfileRepository> _repo = new();
    private readonly Mock<ILogger<SaveVetProfileHandler>> _logger = new();
    private SaveVetProfileHandler Sut() => new(_repo.Object, _logger.Object);

    private static VetProfileRequest ValidRequest() => new(
        "Dr. Joan", "CAT-1", "Clinic", "Addr", "+34600", "j@j.cat");

    [Fact]
    public async Task HandleAsync_ValidRequest_Returns200WithProfile()
    {
        _repo.Setup(r => r.UpsertAsync(It.IsAny<VetProfileRequest>(), It.IsAny<CancellationToken>()))
             .Returns(Task.CompletedTask);

        var result = await Sut().HandleAsync(ValidRequest(), CancellationToken.None);

        var ok = Assert.IsType<Ok<VetProfileResponse>>(result.Result);
        Assert.Equal("Dr. Joan", ok.Value!.Name);
        Assert.Equal("CAT-1", ok.Value.LicenceNumber);
    }

    [Fact]
    public async Task HandleAsync_EmptyName_ReturnsValidationProblem()
    {
        var request = ValidRequest() with { Name = "" };

        var result = await Sut().HandleAsync(request, CancellationToken.None);

        Assert.IsType<ValidationProblem>(result.Result);
        _repo.Verify(r => r.UpsertAsync(It.IsAny<VetProfileRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_ValidRequest_LogsAuditMessage()
    {
        _repo.Setup(r => r.UpsertAsync(It.IsAny<VetProfileRequest>(), It.IsAny<CancellationToken>()))
             .Returns(Task.CompletedTask);

        await Sut().HandleAsync(ValidRequest(), CancellationToken.None);

        _logger.Verify(l => l.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("saved profile")),
            null,
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }
}
