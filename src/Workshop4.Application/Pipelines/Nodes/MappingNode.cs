using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Workshop4.Application.Json.Models;
using Workshop4.Application.Pipelines.Models;
using Workshop4.Application.Pipelines.Presentation;

namespace Workshop4.Application.Pipelines.Nodes;

public sealed class MappingNode : IPipelineNode
{
    private readonly List<MappingNodeProjection> _projections = [];

    public bool IsEnabled { get; set; }

    public IReadOnlyCollection<MappingNodeProjection> Projections => _projections;

    public void AddProjection(MappingNodeProjection projection)
    {
        _projections.Add(projection);
    }

    public void RemoveProjection(MappingNodeProjection projection)
    {
        _projections.Remove(projection);
    }

    public async Task<NodeExecutionResult> ExecuteAsync(
        JsonDocument input,
        IPipelinePresentationManager presentationManager)
    {
        if (IsEnabled is false)
            return new NodeExecutionResult.Success(input);

        await presentationManager.OnExecutingNodeChangedAsync(this);
        await Task.Delay(TimeSpan.FromMilliseconds(500));

        if (input is JsonObjectDocument obj)
        {
            return TryCreateProjectedObject(obj, out JsonObjectDocument? projectedObject)
                ? new NodeExecutionResult.Success(projectedObject)
                : new NodeExecutionResult.Failure("Failed to map object");
        }

        if (input is JsonArrayDocument arr)
        {
            var mappedObjects = new List<JsonObjectDocument>();

            foreach (JsonObjectDocument value in arr.Values)
            {
                if (TryCreateProjectedObject(value, out JsonObjectDocument? projectedObject))
                {
                    mappedObjects.Add(projectedObject);
                }
                else
                {
                    return new NodeExecutionResult.Failure("Failed to map object");
                }
            }

            return new NodeExecutionResult.Success(new JsonArrayDocument(mappedObjects));
        }

        return new NodeExecutionResult.Failure($"Invalid input for mapping operation = {input}");
    }

    private bool TryCreateProjectedObject(
        JsonObjectDocument obj,
        [NotNullWhen(true)] out JsonObjectDocument? projectedObject)
    {
        var properties = new List<JsonProperty>();

        foreach (MappingNodeProjection projection in Projections)
        {
            if (projection.TryProjectProperty(obj, out JsonProperty? property))
            {
                properties.Add(property);
            }
            else
            {
                projectedObject = null;
                return false;
            }
        }

        projectedObject = new JsonObjectDocument(properties);
        return true;
    }

    public override string ToString()
    {
        if (Projections.Count == 0)
            return "Map (no projections)";

        IEnumerable<string> pairs = Projections
            .Select(projection => $"{projection.SourceFieldName}->{projection.TargetFieldName}");

        var joined = string.Join(", ", pairs);
        return $"Map {joined}";
    }
}
