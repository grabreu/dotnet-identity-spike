using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Spike.API.OpenApi;

public class ApiInfoTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Info = new OpenApiInfo
        {
            Title = "Spike API",
            Version = "v1",
            Description = "Spike exploring ASP.NET Core Identity + JWT authentication in a .NET minimal API."
        };

        return Task.CompletedTask;
    }
}
