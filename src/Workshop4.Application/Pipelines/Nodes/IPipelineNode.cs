using Workshop4.Application.Json.Models;
using Workshop4.Application.Pipelines.Models;
using Workshop4.Application.Pipelines.Presentation;

namespace Workshop4.Application.Pipelines.Nodes;

public interface IPipelineNode
{
    public bool IsEnabled { get; set; }

    Task<NodeExecutionResult> ExecuteAsync(JsonDocument input, IPipelinePresentationManager presentationManager);
}
