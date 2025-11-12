using System.Diagnostics.CodeAnalysis;
using Workshop4.Application.Json.Models;

namespace Workshop4.Application.Pipelines.Nodes;

public sealed class MappingNodeProjection
{
    public string SourceFieldName { get; set; } = string.Empty;

    public string TargetFieldName { get; set; } = string.Empty;

    public bool TryProjectProperty(JsonObjectDocument obj, [NotNullWhen(true)] out JsonProperty? property)
    {
        if (string.IsNullOrEmpty(SourceFieldName))
        {
            property = obj.TryGetProperty(TargetFieldName, out JsonProperty? jsonProperty)
                ? jsonProperty
                : null;
        }
        else
        {
            property = obj.TryGetProperty(SourceFieldName, out JsonProperty? jsonProperty)
                ? jsonProperty with { Name = TargetFieldName }
                : null;
        }

        return property is not null;
    }
}
