using Workshop4.Application.Pipelines.Nodes;

namespace Workshop4.Application.Pipelines.NodeFactories;

public interface IPipelineNodeFactory
{
    string Name { get; }

    IPipelineNode CreateNode();
}
