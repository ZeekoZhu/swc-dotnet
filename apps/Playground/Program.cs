// See https://aka.ms/new-console-template for more information

using SwcDotNet;

string ParseTypesScript(string source)
{
    var parser = new SwcParser(new TsParserConfig());
    return parser.Parse(source);
}

var source = "\ntype Foo = { bar: string };\n";
var ast = ParseTypesScript(source);
Console.WriteLine(ast);

// Console.WriteLine(ParseTypesScript(source));
