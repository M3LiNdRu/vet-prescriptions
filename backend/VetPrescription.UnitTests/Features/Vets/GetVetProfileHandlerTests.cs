using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using Moq;
using VetPrescription.Api.Features.Vets.GetVetProfile;
using VetPrescription.Api.Features.Vets.SaveVetProfile;

namespace VetPrescription.UnitTests.Features.Vets;

public class GetVetProfileHandlerTests
{
    private readonly Mock<IGetVetProfileRepository> _repo = new();
    private readonly Mock<ILogger<GetVetProfileHandler>> _logger = new();
    private GetVetProfileHandler Sut() => new(_repo.Object, _logger.Object);

    private static VetProfileResponse SampleProfile() => new(
        "Dr. Joan", "CAT-1", "Clinic", "Addr", "+34600", "j@j.cat");

    [Fact]
    public async Task HandleAsync_ProfileExists_Returns200WithProfile()
    {
        _repo.Setup(r => r.GetAsync(It.IsAny<CancellationToken>()))
             .ReturnsAsync(SampleProfile());

        var result = await Sut().HandleAsync(CancellationToken.None);

        var ok = Assert.IsType<Ok<VetProfileResponse>>(result.Result);
        Assert.Equal("Dr. Joan", ok.Value!.Name);
        Assert.Equal("CAT-1", ok.Value.LicenceNumber);
    }

    [Fact]
    public async Task HandleAsync_NoProfile_Returns404()
    {
        _repo.Setup(r => r.GetAsync(It.IsAny<CancellationToken>()))
             .ReturnsAsync((VetProfileResponse?)null);

        var result = await Sut().HandleAsync(CancellationToken.None);

        Assert.IsType<NotFound>(result.Result);
    }

    [Fact]
    public async Task HandleAsync_ProfileExists_LogsAuditMessage()
    {
        _repo.Setup(r => r.GetAsync(It.IsAny<CancellationToken>()))
             .ReturnsAsync(SampleProfile());

        await Sut().HandleAsync(CancellationToken.None);

        _logger.Verify(l => l.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("retrieved profile")),
            null,
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }
}
