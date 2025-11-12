using Workshop4.Application.Pipelines.Nodes;

namespace Workshop4.Application.Pipelines.NodeFactories;

public sealed class FilterNodeFactory : IPipelineNodeFactory
{
    public string Name => "Filter";

    public IPipelineNode CreateNode()
    {
        return new FilterNode { IsEnabled = true };
    }
}
