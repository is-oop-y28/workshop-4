using Workshop4.Application.Pipelines.Nodes;

namespace Workshop4.Application.Pipelines.NodeFactories;

public sealed class MappingNodeFactory : IPipelineNodeFactory
{
    public string Name => "Map";

    public IPipelineNode CreateNode()
    {
        return new MappingNode { IsEnabled = true };
    }
}
