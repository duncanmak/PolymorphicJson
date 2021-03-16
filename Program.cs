using System;
using System.Text.Json;

class Program
{
    static void Main(string[] args)
    {
        var models = new IBase[] {
            new Foo("john"),
            new Bar(10)
        };

        var options = new JsonSerializerOptions {
            Converters = { new BaseConverter() },
        };

        var json = JsonSerializer.Serialize(models, options);

        Console.WriteLine($"Output: {json}");

        var results = JsonSerializer.Deserialize<IBase[]>(json, options);

        // Console.WriteLine(result);
        foreach (var i in results) Console.WriteLine(i);
    }
}

