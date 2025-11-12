using System.Text.Json.Nodes;
using Workshop4.Application.Json.Models;
using JsonDocument = Workshop4.Application.Json.Models.JsonDocument;
using JsonObjectProperty = Workshop4.Application.Json.Models.JsonProperty;
using JsonValue = System.Text.Json.Nodes.JsonValue;

namespace Workshop4.Application.Json.Factories;

public sealed class JsonDocumentFactory : IJsonDocumentFactory
{
    public JsonDocument CreateDocument(string json)
    {
        var jsonNode = JsonNode.Parse(json);

        if (jsonNode is null)
            throw new InvalidOperationException("Could not parse json");

        return MapNode(jsonNode);
    }

    private static JsonDocument MapNode(JsonNode? node)
    {
        return node switch
        {
            JsonArray jsonArray => new JsonArrayDocument(jsonArray.Select(MapArrayElementNode).ToArray()),

            JsonObject jsonObject => new JsonObjectDocument(jsonObject
                .Select(kvp => new JsonObjectProperty(kvp.Key, MapNode(kvp.Value)))
                .ToArray()),

            JsonValue jsonValue => new Models.JsonValue(jsonValue.ToString()),

            null => new JsonNullDocument(),

            _ => throw new ArgumentOutOfRangeException(nameof(node)),
        };
    }

    private static JsonObjectDocument MapArrayElementNode(JsonNode? node)
    {
        JsonDocument document = MapNode(node);

        if (document is not JsonObjectDocument obj)
            throw new InvalidOperationException("Arrays must contain objects");

        return obj;
    }
}
