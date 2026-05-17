using MongoDB.Bson;
using MongoDB.Driver;
using VetPrescription.Api.Infrastructure;
using VetPrescription.Domain.Entities;

namespace VetPrescription.Api.Features.Prescriptions.GetById;

public interface IGetPrescriptionByIdRepository
{
    Task<Prescription?> GetByIdAsync(string id, CancellationToken ct);
}

public class GetPrescriptionByIdRepository(MongoDbContext db) : IGetPrescriptionByIdRepository
{
    public async Task<Prescription?> GetByIdAsync(string id, CancellationToken ct)
    {
        var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
        var doc = await db.GetCollection<BsonDocument>("prescriptions")
            .Find(filter).FirstOrDefaultAsync(ct);
        return doc is null ? null : ToDomain(doc);
    }

    internal static Prescription ToDomain(BsonDocument doc)
    {
        var vet = doc["vet"].AsBsonDocument;
        var owner = doc["owner"].AsBsonDocument;
        var patient = doc["patient"].AsBsonDocument;
        var items = doc["items"].AsBsonArray
            .Select(i => i.AsBsonDocument)
            .Select(i => new PrescriptionItem
            {
                DrugName = i["drugName"].AsString,
                Quantity = i["quantity"].AsString,
                PharmaceuticalForm = i["pharmaceuticalForm"].AsString,
                DosageRegimen = i["dosageRegimen"].AsString,
                WithdrawalPeriod = i["withdrawalPeriod"].IsBsonNull ? null : i["withdrawalPeriod"].AsString,
            }).ToList();

        return new Prescription
        {
            Id = doc["_id"].AsString,
            PrescriptionNumber = doc["prescriptionNumber"].AsString,
            Date = DateOnly.Parse(doc["date"].AsString),
            Vet = new Vet
            {
                Name = vet["name"].AsString,
                LicenceNumber = vet["licenceNumber"].AsString,
                ClinicName = vet["clinicName"].AsString,
                Address = vet["address"].AsString,
                Phone = vet["phone"].AsString,
                Email = vet["email"].AsString,
            },
            Owner = new Owner
            {
                Name = owner["name"].AsString,
                Address = owner["address"].AsString,
                Phone = owner["phone"].AsString,
                CifDni = owner["cifDni"].AsString,
            },
            Patient = new Patient
            {
                AnimalName = patient["animalName"].AsString,
                Species = patient["species"].AsString,
                Breed = patient["breed"].AsString,
            },
            Items = items,
            Warnings = doc["warnings"].IsBsonNull ? null : doc["warnings"].AsString,
            IsOffLabel = doc["isOffLabel"].AsBoolean,
            IsAntimicrobialSpecialUse = doc["isAntimicrobialSpecialUse"].AsBoolean,
            PdfUrl = doc.Contains("pdfUrl") && !doc["pdfUrl"].IsBsonNull ? doc["pdfUrl"].AsString : null,
        };
    }
}
