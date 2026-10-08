namespace Spike.Users.FunctionalTests.Testing;

[CollectionDefinition(Name)]
public class ApplicationCollection : ICollectionFixture<ApplicationFixture>
{
    public const string Name = "Application";
}
