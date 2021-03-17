using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
#nullable enable

public class BaseConverter : JsonConverter<IBase> {

    public static Dictionary<string, Type> mapping = new();

    public BaseConverter()
    {
        foreach (var t in Assembly.GetExecutingAssembly().GetTypes()) {
            if (t.IsAssignableTo(typeof(IBase))) {
                mapping[t.Name] = t;
            }
        }
    }

    public override bool CanConvert(Type t) => t.IsAssignableFrom(typeof(IBase));

    public override IBase? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var r = reader;

        // Peek into the "type" attribute
        if (r.TokenType != JsonTokenType.StartObject) throw new JsonException("JsonTokenType.StartObject");

        if (!r.Read() || r.TokenType != JsonTokenType.PropertyName) throw new JsonException("JsonTokenType.PropertyName");

        if (r.GetString() != "type") throw new JsonException("type");

        if (!r.Read() || r.TokenType != JsonTokenType.String) throw new JsonException("JsonTokenType.String");

        var t = r.GetString(); mapping.TryGetValue(t ?? "", out Type? type);

        if (type == null) throw new JsonException($"{t} is not a recognized type");

        return (IBase?) JsonSerializer.Deserialize(ref reader, type, options);
    }

    public override void Write(Utf8JsonWriter writer, IBase value, JsonSerializerOptions options)
    {
        var type = value.GetType();
        if (type != null) JsonSerializer.Serialize(writer, value, type, options);
    }
}
