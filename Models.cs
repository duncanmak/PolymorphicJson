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
        var current = reader;

        // Peek into the "type" attribute
        if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException();

        reader.Read();
        if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException();

        if (reader.GetString() != "type") throw new JsonException();

        reader.Read();
        if (reader.TokenType != JsonTokenType.String) throw new JsonException();

        var t = reader.GetString();
        mapping.TryGetValue(t ?? "", out Type? type);

        if (type == null) throw new JsonException($"{t} is not a recognized type");
        // reader.Read();

        Console.WriteLine($"Saw {type}");
        Console.WriteLine($"{reader.TokenStartIndex} {reader.TokenType}");
        Console.WriteLine($"{current.TokenStartIndex} {current.TokenType}");

        var result = (IBase?) JsonSerializer.Deserialize(ref current, type, options);
        return result;
    }

    public override void Write(Utf8JsonWriter writer, IBase value, JsonSerializerOptions options)
    {
        var type = value.GetType();
        if (type != null)
            JsonSerializer.Serialize(writer, value, type, options);
    }
}

public interface IBase {
    public string type { get; }

    // public List<IBase> friends { get; }
}

public class Foo : IBase {
    public string type => "Foo";
    public string name { get; set; } = "";

    // public List<IBase> friends =>
    //     new List<IBase>() { new Bar(100) };

    public override string ToString() => $"Foo(name = {name})";
}

public class Bar : IBase {
    public string type => "Bar";
    public int age { get; set; } = 0;

    public override string ToString() => $"Bar(name = {age})";
    // public List<IBase> friends => new List<IBase>();
}