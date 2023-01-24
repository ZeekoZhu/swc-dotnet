// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using SwcDotNet.Native;

string ParseTypesScript(string s)
{
    var parseOption = new
    {
        syntax = "typescript"
    };
    using var swcWrap = SwcWrap.New(
        new ParseParams
        {
            filename = OptionStringRef.FromNullable(null),
            options = JsonSerializer.Serialize(parseOption),
            src = s
        });
    return swcWrap.Parse();
}


// var source = $"type Foo = {{ bar: string }}; const foo: Foo = {{ bar: '{12}' }};";
for (var i = 0; i < 3_000_000; i++)
{
    var source = $"type Foo = {{ bar: string }}; const foo: Foo = {{ bar: '12' }};";
    // var source = $"type Foo = {{ bar: string }}; const foo: Foo = {{ bar: '{i}' }};";
    ParseTypesScript(source);
}

// Console.WriteLine(ParseTypesScript(source));
