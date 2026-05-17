using MongoDB.Bson;
using MongoDB.Driver;
using VetPrescription.Api.Infrastructure;
using VetPrescription.Domain.Entities;

namespace VetPrescription.Api.Features.Prescriptions.Create;

public interface ICreatePrescriptionRepository
{
    Task<Prescription> InsertAsync(CreatePrescriptionRequest request, CancellationToken ct);
}

public class CreatePrescriptionRepository(MongoDbContext db) : ICreatePrescriptionRepository
{
    public async Task<Prescription> InsertAsync(CreatePrescriptionRequest request, CancellationToken ct)
    {
        var sequence = await GetNextSequenceAsync(ct);
        var id = ObjectId.GenerateNewId().ToString();

        var prescription = Prescription.Create(
            id,
            sequence,
            new Vet
            {
                Name = request.Vet.Name,
                LicenceNumber = request.Vet.LicenceNumber,
                ClinicName = request.Vet.ClinicName,
                Address = request.Vet.Address,
                Phone = request.Vet.Phone,
                Email = request.Vet.Email,
            },
            new Owner
            {
                Name = request.Owner.Name,
                Address = request.Owner.Address,
                Phone = request.Owner.Phone,
                CifDni = request.Owner.CifDni,
            },
            new Patient
            {
                AnimalName = request.Patient.AnimalName,
                Species = request.Patient.Species,
                Breed = request.Patient.Breed,
            },
            request.Items.Select(i => new PrescriptionItem
            {
                DrugName = i.DrugName,
                Quantity = i.Quantity,
                PharmaceuticalForm = i.PharmaceuticalForm,
                DosageRegimen = i.DosageRegimen,
                WithdrawalPeriod = i.WithdrawalPeriod,
            }).ToList(),
            request.Warnings,
            request.IsOffLabel,
            request.IsAntimicrobialSpecialUse);

        var doc = ToBsonDocument(prescription);
        await db.GetCollection<BsonDocument>("prescriptions").InsertOneAsync(doc, cancellationToken: ct);
        return prescription;
    }

    private async Task<int> GetNextSequenceAsync(CancellationToken ct)
    {
        var counters = db.GetCollection<BsonDocument>("counters");
        var filter = Builders<BsonDocument>.Filter.Eq("_id", "prescriptions");
        var update = Builders<BsonDocument>.Update.Inc("seq", 1);
        var options = new FindOneAndUpdateOptions<BsonDocument>
        {
            IsUpsert = true,
            ReturnDocument = ReturnDocument.After,
        };
        var result = await counters.FindOneAndUpdateAsync(filter, update, options, ct);
        return result["seq"].AsInt32;
    }

    private static BsonDocument ToBsonDocument(Prescription p) => new()
    {
        ["_id"] = p.Id,
        ["prescriptionNumber"] = p.PrescriptionNumber,
        ["date"] = p.Date.ToString("yyyy-MM-dd"),
        ["vet"] = new BsonDocument
        {
            ["name"] = p.Vet.Name,
            ["licenceNumber"] = p.Vet.LicenceNumber,
            ["clinicName"] = p.Vet.ClinicName,
            ["address"] = p.Vet.Address,
            ["phone"] = p.Vet.Phone,
            ["email"] = p.Vet.Email,
        },
        ["owner"] = new BsonDocument
        {
            ["name"] = p.Owner.Name,
            ["address"] = p.Owner.Address,
            ["phone"] = p.Owner.Phone,
            ["cifDni"] = p.Owner.CifDni,
        },
        ["patient"] = new BsonDocument
        {
            ["animalName"] = p.Patient.AnimalName,
            ["species"] = p.Patient.Species,
            ["breed"] = p.Patient.Breed,
        },
        ["items"] = new BsonArray(p.Items.Select(i => new BsonDocument
        {
            ["drugName"] = i.DrugName,
            ["quantity"] = i.Quantity,
            ["pharmaceuticalForm"] = i.PharmaceuticalForm,
            ["dosageRegimen"] = i.DosageRegimen,
            ["withdrawalPeriod"] = (BsonValue)i.WithdrawalPeriod ?? BsonNull.Value,
        })),
        ["warnings"] = (BsonValue)p.Warnings ?? BsonNull.Value,
        ["isOffLabel"] = p.IsOffLabel,
        ["isAntimicrobialSpecialUse"] = p.IsAntimicrobialSpecialUse,
        ["pdfUrl"] = BsonNull.Value,
    };
}
