using Workshop4.Application.Json.Models;
using Workshop4.Application.Pipelines.Nodes;

namespace Workshop4.Application.Pipelines.Models;

public sealed record ExecutionFrame(IPipelineNode Node, JsonDocument Input);
