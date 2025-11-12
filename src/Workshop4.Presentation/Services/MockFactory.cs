using Workshop4.Application.Pipelines.Models;
using Workshop4.Application.Pipelines.Nodes;

namespace Workshop4.Presentation.Services;

public static class MockFactory
{
    public static readonly string MockInput = """
    [
      {
        "id": 1,
        "name": "Alice",
        "age": 30,
        "height": 170
      },
      {
        "id": 2,
        "name": "Bob",
        "age": 24,
        "height": 180
      }
    ]
    """;

    public static GroupNode CreatePipeline()
    {
        var pipeline = new GroupNode();

        var mapNode = new MappingNode { IsEnabled = true };

        mapNode.AddProjection(new MappingNodeProjection
        {
            SourceFieldName = "id",
            TargetFieldName = "userId",
        });

        mapNode.AddProjection(new MappingNodeProjection
        {
            SourceFieldName = "name",
            TargetFieldName = "displayName",
        });

        mapNode.AddProjection(new MappingNodeProjection
        {
            TargetFieldName = "height",
        });

        var filterNode = new FilterNode
        {
            IsEnabled = true,
            PropertyName = "age",
            FilterOperation = FilterOperation.GreaterThan,
            Value = "25",
        };

        var groupNode = new GroupNode { IsEnabled = true, Name = "Users" };

        pipeline.AddNode(filterNode);
        pipeline.AddNode(mapNode);
        pipeline.AddNode(groupNode);

        return pipeline;
    }
}
