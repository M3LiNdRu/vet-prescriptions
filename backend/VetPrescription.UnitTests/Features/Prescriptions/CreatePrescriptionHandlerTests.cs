using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using Moq;
using VetPrescription.Api.Features.Prescriptions.Create;
using VetPrescription.Domain.Entities;

namespace VetPrescription.UnitTests.Features.Prescriptions;

public class CreatePrescriptionHandlerTests
{
    private readonly Mock<ICreatePrescriptionRepository> _repo = new();
    private readonly Mock<ILogger<CreatePrescriptionHandler>> _logger = new();
    private CreatePrescriptionHandler Sut() => new(_repo.Object, _logger.Object);

    private static CreatePrescriptionRequest ValidRequest() => new(
        new VetRequest("Dr. Joan", "CAT-1", "Clinic", "Addr", "+34600", "j@j.cat"),
        new OwnerRequest("Owner", "Addr", "+34600", "12345678A"),
        new PatientRequest("Rex", "Canis", "Labrador"),
        [new PrescriptionItemRequest("DrugA", "1 box", "Tablets", "1/day for 7d", null)],
        null, false, false);

    [Fact]
    public async Task HandleAsync_ValidRequest_Returns201WithIds()
    {
        var prescription = Prescription.Create("id1", 1,
            new Vet { Name = "Dr. Joan", LicenceNumber = "CAT-1", ClinicName = "Clinic", Address = "Addr", Phone = "+34600", Email = "j@j.cat" },
            new Owner { Name = "Owner", Address = "Addr", Phone = "+34600", CifDni = "12345678A" },
            new Patient { AnimalName = "Rex", Species = "Canis", Breed = "Labrador" },
            [new PrescriptionItem { DrugName = "DrugA", Quantity = "1 box", PharmaceuticalForm = "Tablets", DosageRegimen = "1/day for 7d" }],
            null, false, false);

        _repo.Setup(r => r.InsertAsync(It.IsAny<CreatePrescriptionRequest>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(prescription);

        var result = await Sut().HandleAsync(ValidRequest(), CancellationToken.None);

        var created = Assert.IsType<Created<CreatePrescriptionResponse>>(result.Result);
        Assert.Equal(201, created.StatusCode);
        Assert.Equal("id1", created.Value!.Id);
        Assert.StartsWith("RX-", created.Value.PrescriptionNumber);
    }

    [Fact]
    public async Task HandleAsync_EmptyVetName_ReturnsValidationProblem()
    {
        var request = ValidRequest() with
        {
            Vet = new VetRequest("", "CAT-1", "Clinic", "Addr", "+34600", "j@j.cat")
        };

        var result = await Sut().HandleAsync(request, CancellationToken.None);

        Assert.IsType<ValidationProblem>(result.Result);
        _repo.Verify(r => r.InsertAsync(It.IsAny<CreatePrescriptionRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_EmptyItems_ReturnsValidationProblem()
    {
        var request = ValidRequest() with { Items = [] };

        var result = await Sut().HandleAsync(request, CancellationToken.None);

        Assert.IsType<ValidationProblem>(result.Result);
    }

    [Fact]
    public async Task HandleAsync_ValidRequest_LogsAuditMessage()
    {
        var prescription = Prescription.Create("id1", 1,
            new Vet { Name = "Dr. Joan", LicenceNumber = "CAT-1", ClinicName = "Clinic", Address = "Addr", Phone = "+34600", Email = "j@j.cat" },
            new Owner { Name = "Owner", Address = "Addr", Phone = "+34600", CifDni = "12345678A" },
            new Patient { AnimalName = "Rex", Species = "Canis", Breed = "Labrador" },
            [new PrescriptionItem { DrugName = "DrugA", Quantity = "1 box", PharmaceuticalForm = "Tablets", DosageRegimen = "1/day for 7d" }],
            null, false, false);

        _repo.Setup(r => r.InsertAsync(It.IsAny<CreatePrescriptionRequest>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(prescription);

        await Sut().HandleAsync(ValidRequest(), CancellationToken.None);

        _logger.Verify(l => l.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("issued prescription")),
            null,
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }
}
