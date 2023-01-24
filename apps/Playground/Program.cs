// See https://aka.ms/new-console-template for more information

using SwcDotNet;

string ParseTypesScript(string source)
{
    var parser = new SwcParser(new TsParserConfig());
    return parser.Parse(source);
}

var source = "type Foo = { bar: string }; const foo: Foo = { bar: '12' };";
for (var i = 0; i < 3_000_000; i++) ParseTypesScript(source);

// Console.WriteLine(ParseTypesScript(source));
