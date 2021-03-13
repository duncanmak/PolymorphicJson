using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

class Program
{
    static void Main(string[] args)
    {
        var models = new IBase[] { new Foo { name = "john" }, new Bar { age = 10 } };
        var options = new JsonSerializerOptions {
            Converters = { new BaseConverter() }
        };

        var json = JsonSerializer.Serialize(models, options);

        Console.WriteLine($"Output: {json}");

        var result = JsonSerializer.Deserialize<IBase[]>(json, options);

        // Console.WriteLine(result);
        foreach (var i in result) Console.WriteLine(i);
    }
}

