using MongoDB.Bson;
using MongoDB.Driver;
using VetPrescription.Api.Infrastructure;

namespace VetPrescription.Api.Features.Vets.SaveVetProfile;

public interface ISaveVetProfileRepository
{
    Task UpsertAsync(VetProfileRequest request, CancellationToken ct);
}

public class SaveVetProfileRepository(MongoDbContext db) : ISaveVetProfileRepository
{
    private const string SingletonId = "singleton";

    public async Task UpsertAsync(VetProfileRequest request, CancellationToken ct)
    {
        var collection = db.GetCollection<BsonDocument>("vet_profiles");
        var filter = Builders<BsonDocument>.Filter.Eq("_id", SingletonId);
        var doc = new BsonDocument
        {
            ["_id"] = SingletonId,
            ["name"] = request.Name,
            ["licenceNumber"] = request.LicenceNumber,
            ["clinicName"] = request.ClinicName,
            ["address"] = request.Address,
            ["phone"] = request.Phone,
            ["email"] = request.Email,
        };
        var options = new ReplaceOptions { IsUpsert = true };
        await collection.ReplaceOneAsync(filter, doc, options, ct);
    }
}
