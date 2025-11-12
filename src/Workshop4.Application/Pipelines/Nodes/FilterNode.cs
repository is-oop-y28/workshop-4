using System.Diagnostics;
using Workshop4.Application.Json.Models;
using Workshop4.Application.Pipelines.Models;
using Workshop4.Application.Pipelines.Presentation;

namespace Workshop4.Application.Pipelines.Nodes;

public sealed class FilterNode : IPipelineNode
{
    public FilterNode()
    {
        PropertyName = string.Empty;
        FilterOperation = FilterOperation.None;
        Value = string.Empty;
    }

    public bool IsEnabled { get; set; }

    public string PropertyName { get; set; }

    public FilterOperation FilterOperation { get; set; }

    public string Value { get; set; }

    public async Task<NodeExecutionResult> ExecuteAsync(
        JsonDocument input,
        IPipelinePresentationManager presentationManager)
    {
        if (IsEnabled is false)
            return new NodeExecutionResult.Success(input);

        await presentationManager.OnExecutingNodeChangedAsync(this);
        await Task.Delay(TimeSpan.FromMilliseconds(500));

        if (input is JsonObjectDocument objectDocument)
        {
            NodeFilterResult result = FilterSingleNode(objectDocument);

            if (result is NodeFilterResult.Success success)
            {
                return new NodeExecutionResult.Success(success.IsApplicable
                    ? input
                    : new JsonNullDocument());
            }

            if (result is NodeFilterResult.Error error)
            {
                return new NodeExecutionResult.Failure(error.ErrorMessage);
            }

            throw new UnreachableException($"Unknown result = {result}");
        }

        if (input is JsonArrayDocument arrayDocument)
        {
            var newObjects = new List<JsonObjectDocument>();

            foreach (JsonObjectDocument obj in arrayDocument.Values)
            {
                NodeFilterResult result = FilterSingleNode(obj);

                if (result is NodeFilterResult.Success success)
                {
                    if (success.IsApplicable)
                        newObjects.Add(obj);
                }
                else if (result is NodeFilterResult.Error error)
                {
                    return new NodeExecutionResult.Failure(error.ErrorMessage);
                }
                else
                {
                    throw new UnreachableException($"Unknown result = {result}");
                }
            }

            return new NodeExecutionResult.Success(new JsonArrayDocument(newObjects));
        }

        return new NodeExecutionResult.Failure($"Invalid input for filtering operation = {input}");
    }

    private NodeFilterResult FilterSingleNode(JsonObjectDocument obj)
    {
        if (obj.TryGetProperty(PropertyName, out JsonProperty? property) is false)
        {
            return new NodeFilterResult.Error($"Object does not contain property '{PropertyName}'");
        }

        if (property.Value is not JsonValue propertyValue)
        {
            return new NodeFilterResult.Error("Object property does not contain value");
        }

        bool filterTrue =
            decimal.TryParse(propertyValue.Value, out decimal leftNumber)
            && decimal.TryParse(Value, out decimal rightNumber)
            && (OperationExecutor<decimal>.ExecuteOperation(leftNumber, rightNumber, FilterOperation)
                || OperationExecutor<string>.ExecuteOperation(propertyValue.Value, Value, FilterOperation));

        return new NodeFilterResult.Success(filterTrue);
    }

    private static class OperationExecutor<T>
        where T : IEquatable<T>, IComparable<T>
    {
        public static bool ExecuteOperation(T left, T right, FilterOperation operation)
        {
            return operation switch
            {
                FilterOperation.None => true,
                FilterOperation.Equals => left.Equals(right),
                FilterOperation.NotEquals => left.Equals(right) is false,
                FilterOperation.GreaterThan => left.CompareTo(right) > 0,
                FilterOperation.LessThan => left.CompareTo(right) < 0,
                _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null),
            };
        }
    }

    private abstract record NodeFilterResult
    {
        private NodeFilterResult() { }

        public sealed record Success(bool IsApplicable) : NodeFilterResult;

        public sealed record Error(string ErrorMessage) : NodeFilterResult;
    }

    public override string ToString()
    {
        string op = FilterOperation switch
        {
            FilterOperation.Equals => "=",
            FilterOperation.NotEquals => "!=",
            FilterOperation.GreaterThan => ">",
            FilterOperation.LessThan => "<",
            _ => "?",
        };

        string field = string.IsNullOrWhiteSpace(PropertyName) ? "(field)" : PropertyName;
        string val = string.IsNullOrWhiteSpace(Value) ? "(value)" : Value;
        return $"Filter {field} {op} {val}";
    }
}
