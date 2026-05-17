using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using Moq;
using VetPrescription.Api.Features.Prescriptions.GetById;
using VetPrescription.Domain.Entities;

namespace VetPrescription.UnitTests.Features.Prescriptions;

public class GetPrescriptionByIdHandlerTests
{
    private readonly Mock<IGetPrescriptionByIdRepository> _repo = new();
    private readonly Mock<ILogger<GetPrescriptionByIdHandler>> _logger = new();
    private GetPrescriptionByIdHandler Sut() => new(_repo.Object, _logger.Object);

    private static Prescription SamplePrescription() => Prescription.Create(
        "id1", 1,
        new Vet { Name = "Dr. Joan", LicenceNumber = "CAT-1", ClinicName = "Clinic", Address = "Addr", Phone = "+34600", Email = "j@j.cat" },
        new Owner { Name = "Owner", Address = "Addr", Phone = "+34600", CifDni = "12345678A" },
        new Patient { AnimalName = "Rex", Species = "Canis", Breed = "Labrador" },
        [new PrescriptionItem { DrugName = "DrugA", Quantity = "1 box", PharmaceuticalForm = "Tablets", DosageRegimen = "1/day" }],
        null, false, false);

    [Fact]
    public async Task HandleAsync_ExistingId_Returns200WithFullDetail()
    {
        _repo.Setup(r => r.GetByIdAsync("id1", It.IsAny<CancellationToken>()))
             .ReturnsAsync(SamplePrescription());

        var result = await Sut().HandleAsync("id1", CancellationToken.None);

        var ok = Assert.IsType<Ok<PrescriptionDetailResponse>>(result.Result);
        Assert.Equal("id1", ok.Value!.Id);
        Assert.Equal("Dr. Joan", ok.Value.Vet.Name);
        Assert.Equal("Rex", ok.Value.Patient.AnimalName);
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
    public async Task HandleAsync_ExistingId_LogsAuditMessage()
    {
        _repo.Setup(r => r.GetByIdAsync("id1", It.IsAny<CancellationToken>()))
             .ReturnsAsync(SamplePrescription());

        await Sut().HandleAsync("id1", CancellationToken.None);

        _logger.Verify(l => l.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("retrieved prescription")),
            null,
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }
}
