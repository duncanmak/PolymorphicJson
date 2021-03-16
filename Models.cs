using System;
using System.Linq;

#nullable enable

public static class Extensions {
    public static string ToPrintString(this IBase[] instances) =>
        (instances.Length == 0)
        ? "()"
        : String.Join(", ", instances.Select(i => i.ToString()));
}

public interface IBase {
    public string type { get; }
    public IBase[] friends { get; }
}

public class Foo : IBase {
    public string type => "Foo";
    public Foo(string name) { this.name = name; }

    public string name { get; set; } = "";

    public IBase[] friends =>
        new IBase[] { new Bar(100) };

    public override string ToString() => $"Foo(name = {name}, friends = {friends.ToPrintString()})";
}

public class Bar : IBase {
    public string type => "Bar";

    public Bar(int age) { this.age = age; }

    public int age { get; set; } = 0;

    public override string ToString() => $"Bar(age = {age}, friends = {friends.ToPrintString()})";

    public IBase[] friends => new IBase[] {};
}