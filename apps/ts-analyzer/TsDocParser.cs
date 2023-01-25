using System.Text.Json;
using SwcDotNet;
using Zeeko.TsAnalyzer.Model;

namespace Zeeko.TsAnalyzer;

public class TsDocParser : ICodeDocumentParser
{
    public CodeDocument Parse(string fileName, string code)
    {
        var lineNumberTransformer = new LineNumberTransformer(code);
        var astParser = new SwcParser(new TsParserConfig { Tsx = true, Decorators = true, DynamicImport = false });
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
                    var start = aliasNode.GetProperty("span").GetProperty("start").GetInt32();
                    CodeLocation startLocation = lineNumberTransformer.ToLocation(start);
                    var end = aliasNode.GetProperty("span").GetProperty("end").GetInt32();
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
    private readonly string _code;

    public LineNumberTransformer(string code)
    {
        _code = code;
    }

    public CodeLocation ToLocation(int position)
    {
        var target = position - 1;
        uint line = 1;
        uint column = 1;
        for (var i = 0; i < _code.Length; i++)
        {
            if (i == target)
            {
                return new CodeLocation(line, column);
            }

            var c = _code[i];
            if (c == '\n')
            {
                line++;
                column = 1;
            }
            else if (c == '\r')
            {
                // just ignore \r
            }
            else
            {
                column++;
            }
        }

        throw new ArgumentOutOfRangeException(nameof(position), $"Position '{position}' is out of range");
    }
}
