var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var usersDb = sql.AddDatabase("UsersDb");

builder.AddProject<Projects.Spike_Api>("api")
    .WithReference(usersDb)
    .WaitFor(usersDb);

var app = builder.Build();

await app.RunAsync();
