using MongoDB.Bson;
using MongoDB.Driver;
using VetPrescription.Api.Infrastructure;

namespace VetPrescription.Api.Features.Prescriptions.GetPrescriptionPdfUrl;

public interface IGetPrescriptionPdfUrlRepository
{
    Task<(string? PrescriptionNumber, string? PdfUrl)?> GetPdfInfoAsync(string id, CancellationToken ct);
}

public class GetPrescriptionPdfUrlRepository(MongoDbContext db) : IGetPrescriptionPdfUrlRepository
{
    public async Task<(string? PrescriptionNumber, string? PdfUrl)?> GetPdfInfoAsync(string id, CancellationToken ct)
    {
        var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
        var projection = Builders<BsonDocument>.Projection
            .Include("prescriptionNumber")
            .Include("pdfUrl");

        var doc = await db.GetCollection<BsonDocument>("prescriptions")
            .Find(filter).Project(projection).FirstOrDefaultAsync(ct);

        if (doc is null) return null;

        var pdfUrl = doc.Contains("pdfUrl") && !doc["pdfUrl"].IsBsonNull
            ? doc["pdfUrl"].AsString
            : null;

        return (doc["prescriptionNumber"].AsString, pdfUrl);
    }
}
