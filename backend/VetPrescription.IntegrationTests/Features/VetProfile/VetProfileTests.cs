using System.Net.Http.Json;
using VetPrescription.Api.Features.Vets.SaveVetProfile;

namespace VetPrescription.IntegrationTests.Features.VetProfile;

[Collection(IntegrationTestCollection.Name)]
public class VetProfileTests
{
    private readonly HttpClient _client;

    public VetProfileTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private static VetProfileRequest ValidProfile() => new(
        "Dr. Joan", "CAT-1", "Clinic", "Addr", "+34600", "j@j.cat");

    [Fact]
    public async Task SaveVetProfile_WithValidRequest_Returns200()
    {
        var response = await _client.PostAsJsonAsync("/api/vets/profile", ValidProfile());

        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<VetProfileResponse>();
        Assert.NotNull(body);
        Assert.Equal("Dr. Joan", body.Name);
        Assert.Equal("CAT-1", body.LicenceNumber);
    }

    [Fact]
    public async Task SaveVetProfile_WithEmptyName_Returns400()
    {
        var request = ValidProfile() with { Name = "" };

        var response = await _client.PostAsJsonAsync("/api/vets/profile", request);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetVetProfile_AfterSaving_Returns200WithProfile()
    {
        await _client.PostAsJsonAsync("/api/vets/profile", ValidProfile());

        var response = await _client.GetAsync("/api/vets/profile");

        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<VetProfileResponse>();
        Assert.NotNull(body);
        Assert.Equal("Dr. Joan", body.Name);
    }
}
