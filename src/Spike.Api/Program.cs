using Spike.Api.Endpoints;
using Spike.Api.Infrastructure;
using Spike.Application;
using Spike.Infrastructure;
using Spike.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<OpenApiVersioningTransformer>();
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
    options.AddOperationTransformer<BearerSecuritySchemeTransformer>();
});

builder.AddApplicationServices();
builder.AddInfrastructureServices();

var app = builder.Build();

app.UseExceptionHandler();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapAuthEndpoints();
app.MapUsersEndpoints();

if (app.Environment.IsDevelopment())
{
    await app.EnsureDatabaseAsync();
}

await app.RunAsync();
