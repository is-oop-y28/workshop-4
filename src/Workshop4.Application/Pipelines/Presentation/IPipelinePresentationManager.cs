using Workshop4.Application.Pipelines.Nodes;

namespace Workshop4.Application.Pipelines.Presentation;

public interface IPipelinePresentationManager
{
    Task OnExecutingNodeChangedAsync(IPipelineNode node);
}
