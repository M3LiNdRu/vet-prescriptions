namespace VetPrescription.IntegrationTests;

[CollectionDefinition(Name)]
public class IntegrationTestCollection : ICollectionFixture<TestWebApplicationFactory>
{
    public const string Name = "Integration";
}
