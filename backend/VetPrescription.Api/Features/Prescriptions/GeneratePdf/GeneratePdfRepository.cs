using MongoDB.Bson;
using MongoDB.Driver;
using VetPrescription.Api.Infrastructure;
using VetPrescription.Domain.Entities;

namespace VetPrescription.Api.Features.Prescriptions.GeneratePdf;

public interface IGeneratePdfRepository
{
    Task<Prescription?> GetByIdAsync(string id, CancellationToken ct);
    Task UpdatePdfUrlAsync(string id, string pdfUrl, CancellationToken ct);
}

public class GeneratePdfRepository(MongoDbContext db) : IGeneratePdfRepository
{
    public async Task<Prescription?> GetByIdAsync(string id, CancellationToken ct)
    {
        var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
        var doc = await db.GetCollection<BsonDocument>("prescriptions")
            .Find(filter).FirstOrDefaultAsync(ct);
        return doc is null ? null : GetById.GetPrescriptionByIdRepository.ToDomain(doc);
    }

    public async Task UpdatePdfUrlAsync(string id, string pdfUrl, CancellationToken ct)
    {
        var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
        var update = Builders<BsonDocument>.Update.Set("pdfUrl", pdfUrl);
        await db.GetCollection<BsonDocument>("prescriptions").UpdateOneAsync(filter, update, cancellationToken: ct);
    }
}
