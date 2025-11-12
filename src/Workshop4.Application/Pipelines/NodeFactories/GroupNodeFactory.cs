using Workshop4.Application.Pipelines.Nodes;

namespace Workshop4.Application.Pipelines.NodeFactories;

public sealed class GroupNodeFactory : IPipelineNodeFactory
{
    public string Name => "Group";

    public IPipelineNode CreateNode()
    {
        return new GroupNode { IsEnabled = true, Name = "Group" };
    }
}
