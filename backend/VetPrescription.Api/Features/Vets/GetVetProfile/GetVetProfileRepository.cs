using MongoDB.Bson;
using MongoDB.Driver;
using VetPrescription.Api.Infrastructure;
using VetPrescription.Api.Features.Vets.SaveVetProfile;

namespace VetPrescription.Api.Features.Vets.GetVetProfile;

public interface IGetVetProfileRepository
{
    Task<VetProfileResponse?> GetAsync(CancellationToken ct);
}

public class GetVetProfileRepository(MongoDbContext db) : IGetVetProfileRepository
{
    public async Task<VetProfileResponse?> GetAsync(CancellationToken ct)
    {
        var collection = db.GetCollection<BsonDocument>("vet_profiles");
        var filter = Builders<BsonDocument>.Filter.Eq("_id", "singleton");
        var doc = await collection.Find(filter).FirstOrDefaultAsync(ct);
        if (doc is null) return null;

        return new VetProfileResponse(
            doc["name"].AsString,
            doc["licenceNumber"].AsString,
            doc["clinicName"].AsString,
            doc["address"].AsString,
            doc["phone"].AsString,
            doc["email"].AsString);
    }
}
