using System.Text.Json.Nodes;
using Workshop4.Application.Json.Models;
using JsonValue = Workshop4.Application.Json.Models.JsonValue;

namespace Workshop4.Application.Json;

public static class JsonDocumentFormatter
{
    public static string FormatDocument(JsonDocument doc)
    {
        return ToJsonNode(doc)?.ToString() ?? "null";
    }

    private static JsonNode? ToJsonNode(JsonDocument document)
    {
        return document switch
        {
            JsonValue jsonValue => System.Text.Json.Nodes.JsonValue.Create(jsonValue.Value),
            JsonNullDocument => null,

            JsonArrayDocument arr => new JsonArray(arr.Values.Select(ToJsonNode).ToArray()),

            JsonObjectDocument obj => new JsonObject(
                obj.Properties.Select(prop => new KeyValuePair<string, JsonNode?>(prop.Name, ToJsonNode(prop.Value)))),

            _ => throw new ArgumentOutOfRangeException(nameof(document)),
        };
    }
}
