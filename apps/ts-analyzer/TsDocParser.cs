using System.Text.Json;
using SwcDotNet;
using Zeeko.TsAnalyzer.Model;

namespace Zeeko.TsAnalyzer;

public class TsDocParser : ICodeDocumentParser
{
    public CodeDocument Parse(string fileName, string code)
    {
        using var astParser =
            new SwcParser(new TsParserConfig { Tsx = true, Decorators = true, DynamicImport = false });
        var lineNumberTransformer = new LineNumberTransformer(astParser);
        var astJson = astParser.Parse(code, fileName);
        var ast = JsonDocument.Parse(astJson);
        if (ast is null)
        {
            throw new DocumentParseException("Failed to parse AST");
        }

        var declarations = ast.RootElement.GetProperty("body")
            .EnumerateArray()
            .Where(node => node.GetProperty("type").GetString() == "TsTypeAliasDeclaration")
            .Select(
                aliasNode =>
                {
                    var name = aliasNode.GetProperty("id").GetProperty("value").GetString()!;
                    var start = aliasNode.GetProperty("span").GetProperty("start").GetUInt32();
                    CodeLocation startLocation = lineNumberTransformer.ToLocation(start);
                    var end = aliasNode.GetProperty("span").GetProperty("end").GetUInt32();
                    CodeLocation endLocation = lineNumberTransformer.ToLocation(end);
                    var identifier = new Identifier(name, fileName, startLocation, endLocation);
                    var declaration = new Declaration
                    {
                        File = fileName,
                        Identifier = identifier,
                        References = new List<Identifier>()
                    };
                    return declaration;
                })
            .ToList();
        return new CodeDocument
        {
            Declarations = declarations,
            Imports = new List<Identifier>()
        };
    }
}

public class LineNumberTransformer
{
    private readonly SwcParser _parser;

    public LineNumberTransformer(SwcParser parser)
    {
        _parser = parser;
    }

    public CodeLocation ToLocation(uint position)
    {
        var (line, column) = _parser.GetPosition(position);
        return new CodeLocation((uint)line, (uint)column + 1);
    }
}
