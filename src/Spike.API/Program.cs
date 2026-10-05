using Scalar.AspNetCore;
using Serilog;
using Spike.API.OpenApi;
using Spike.Notifications;
using Spike.Users;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<ApiInfoTransformer>();
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
    options.AddOperationTransformer<BearerSecuritySchemeTransformer>();
});

builder.Services.AddMediator(options => options.ServiceLifetime = ServiceLifetime.Scoped);

builder.Services.AddUsersModuleServices(builder.Configuration);
builder.Services.AddNotificationsModuleServices();

var app = builder.Build();

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapUsersModuleEndpoints();

if (app.Environment.IsDevelopment())
{
    await app.InitializeUsersModuleAsync();
}

await app.RunAsync();
