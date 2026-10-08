namespace Spike.Api.Infrastructure;

public class OpenApiVersioningTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Info = new OpenApiInfo
        {
            Title = "Spike API",
            Version = "v1",
            Description = "Spike exploring ASP.NET Core Identity + JWT authentication in a .NET minimal API.",
            Contact = new OpenApiContact
            {
                Name = "Gabriel Abreu",
                Url = new Uri("https://grabreu.dev")
            }
        };

        return Task.CompletedTask;
    }
}
