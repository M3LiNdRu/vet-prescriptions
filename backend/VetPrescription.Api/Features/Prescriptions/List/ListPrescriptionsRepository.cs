using MongoDB.Bson;
using MongoDB.Driver;
using VetPrescription.Api.Infrastructure;

namespace VetPrescription.Api.Features.Prescriptions.List;

public interface IListPrescriptionsRepository
{
    Task<IReadOnlyList<PrescriptionSummaryResponse>> GetAllAsync(CancellationToken ct);
}

public class ListPrescriptionsRepository(MongoDbContext db) : IListPrescriptionsRepository
{
    public async Task<IReadOnlyList<PrescriptionSummaryResponse>> GetAllAsync(CancellationToken ct)
    {
        var collection = db.GetCollection<BsonDocument>("prescriptions");
        var sort = Builders<BsonDocument>.Sort.Descending("date");
        var projection = Builders<BsonDocument>.Projection
            .Include("_id")
            .Include("prescriptionNumber")
            .Include("date")
            .Include("patient.animalName")
            .Include("vet.name");

        var docs = await collection
            .Find(FilterDefinition<BsonDocument>.Empty)
            .Sort(sort)
            .Project(projection)
            .ToListAsync(ct);

        return docs.Select(d => new PrescriptionSummaryResponse(
            d["_id"].AsString,
            d["prescriptionNumber"].AsString,
            d["date"].AsString,
            d["patient"]["animalName"].AsString,
            d["vet"]["name"].AsString
        )).ToList();
    }
}
