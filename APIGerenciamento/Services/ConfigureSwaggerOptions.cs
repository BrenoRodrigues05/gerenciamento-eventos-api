using Asp.Versioning;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
    }

    public void Configure(SwaggerGenOptions options)
    {
        // Cria um SwaggerDoc para cada versão registrada
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, new OpenApiInfo
            {
                Title = "APIGerenciamento",
                Version = description.ApiVersion.ToString(),
                Description = description.IsDeprecated
                    ? "Esta versão está obsoleta."
                    : "API de gerenciamento de eventos"
            });
        }

        // Inclui comentários XML se houver
        // var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        // var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        // options.IncludeXmlComments(xmlPath);

        // Filtro para vincular endpoints à versão correta
        options.DocInclusionPredicate((docName, apiDesc) =>
        {
            if (!apiDesc.TryGetMethodInfo(out var methodInfo))
                return false;

            var versions = apiDesc.ActionDescriptor.EndpointMetadata
                .OfType<ApiVersionAttribute>()
                .SelectMany(attr => attr.Versions)
                .Union(
                    apiDesc.ActionDescriptor.EndpointMetadata
                        .OfType<MapToApiVersionAttribute>()
                        .SelectMany(attr => attr.Versions)
                )
                .Select(v => $"v{v}");

            return versions.Any(v => v == docName);
        });
    }
}
