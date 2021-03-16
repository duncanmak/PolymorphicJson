using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
#nullable enable

public class BaseConverter : JsonConverter<IBase> {

    public static Dictionary<string, Type> mapping = new() {
        ["Foo"] = typeof(Foo),
        ["Bar"] = typeof(Bar)
    };

    public override bool CanConvert(Type type) => type.IsAssignableFrom(typeof(IBase));

    public override IBase? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return ReadBase(ref reader, typeToConvert, options);
    }

    public IBase? ReadFoo(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return (IBase?) JsonSerializer.Deserialize(ref reader, typeof(Foo), options);
    }

    public IBase? ReadBase(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
		var r = reader;

        // Peek into the "type" attribute
        if (r.TokenType != JsonTokenType.StartObject) throw new JsonException();

        r.Read();
        if (r.TokenType != JsonTokenType.PropertyName) throw new JsonException();

        if (r.GetString() != "type") throw new JsonException();

        r.Read();
        if (r.TokenType != JsonTokenType.String) throw new JsonException();

        var t = r.GetString();
        mapping.TryGetValue(t ?? "", out Type? type);
        if (type == null) throw new JsonException($"{t} is not a recognized type");

		return (IBase?) JsonSerializer.Deserialize(ref reader, type, options);
    }

    public override void Write(Utf8JsonWriter writer, IBase value, JsonSerializerOptions options)
    {
        var type = value.GetType();
        if (type != null)
            JsonSerializer.Serialize(writer, value, type, options);
    }
}
