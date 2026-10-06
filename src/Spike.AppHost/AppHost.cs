var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var usersDb = sql.AddDatabase("UsersDb");

var mailpit = builder.AddMailPit("mailpit");

builder.AddProject<Projects.Spike_Api>("api")
    .WithReference(usersDb)
    .WithEnvironment("Email__Host", mailpit.Resource.PrimaryEndpoint.Property(EndpointProperty.Host))
    .WithEnvironment("Email__Port", mailpit.Resource.PrimaryEndpoint.Property(EndpointProperty.Port))
    .WaitFor(usersDb)
    .WaitFor(mailpit);

var app = builder.Build();

await app.RunAsync();
