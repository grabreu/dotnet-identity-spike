using Spike.Api.Infrastructure;
using Spike.Notifications;
using Spike.ServiceDefaults;
using Spike.Contracts;
using Spike.Users;
using Spike.Users.Behaviors;

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

builder.Services.AddMediator(options =>
{
    options.ServiceLifetime = ServiceLifetime.Scoped;
    options.Assemblies = [typeof(UsersModule), typeof(NotificationsModule), typeof(EmailSignInStartedEvent)];
    options.PipelineBehaviors = [typeof(ValidationBehavior<,>)];
});

builder.AddUsersModule();
builder.AddNotificationsModule();

var app = builder.Build();

app.UseExceptionHandler();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapUsersEndpoints();

if (app.Environment.IsDevelopment())
{
    await app.EnsureUsersDatabaseAsync();
}

await app.RunAsync();
