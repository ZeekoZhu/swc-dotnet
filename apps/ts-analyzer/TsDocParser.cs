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

        var declarationCollector = new TsDeclarationCollector(lineNumberTransformer, fileName);
        declarationCollector.Visit(ast);

        return new CodeDocument
        {
            Declarations = declarationCollector.Declarations,
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

/// <summary>
/// A dfs tree visitor for json document
/// </summary>
public abstract class JsonTreeVisitor
{
    protected readonly Stack<JsonElement> WaitList = new();
    public abstract bool VisitObject(JsonElement jsonObj);
    public abstract bool VisitArray(JsonElement jsonArray);

    protected void OnObject(JsonElement obj)
    {
        var keepGoing = VisitObject(obj);
        if (!keepGoing)
        {
            return;
        }

        foreach (var prop in obj.EnumerateObject())
        {
            WaitList.Push(prop.Value);
        }
    }

    public void Visit(JsonDocument doc)
    {
        WaitList.Clear();
        WaitList.Push(doc.RootElement);
        while (WaitList.TryPop(out var node))
        {
            switch (node.ValueKind)
            {
                case JsonValueKind.Object:
                    OnObject(node);
                    break;
                case JsonValueKind.Array:
                    OnArray(node);
                    break;
            }
        }
    }

    protected void OnArray(JsonElement it)
    {
        var keepGoing = VisitArray(it);
        if (!keepGoing)
        {
            return;
        }

        foreach (var el in it.EnumerateArray())
        {
            WaitList.Push(el!);
        }
    }
}

public class TsDeclarationCollector : JsonTreeVisitor
{
    private readonly LineNumberTransformer _lnt;
    private readonly string _fileName;

    public TsDeclarationCollector(LineNumberTransformer lineNumberTransformer, string fileName)
    {
        _lnt = lineNumberTransformer;
        _fileName = fileName;
    }

    public List<Declaration> Declarations { get; } = new();

    public override bool VisitObject(JsonElement jsonObj)
    {
        if (jsonObj.TryGetProperty("type", out var typeProp))
        {
            var type = typeProp.GetString();
            switch (type)
            {
                case "TsTypeAliasDeclaration":
                    AddTypeAlias(jsonObj);
                    return false;
                case "VariableDeclarator":
                    AddVariable(jsonObj);
                    return false;
            }
        }

        return true;
    }

    public override bool VisitArray(JsonElement jsonArray)
    {
        return true;
    }

    private void AddVariable(JsonElement node)
    {
        Declarations.Add(CreateDeclaration(ToIdentifier(node)));
    }

    private void AddTypeAlias(JsonElement aliasNode)
    {
        Declarations.Add(CreateDeclaration(ToIdentifier(aliasNode)));
    }

    private Declaration CreateDeclaration(Identifier identifier)
    {
        var declaration = new Declaration
        {
            File = _fileName,
            Identifier = identifier,
            References = new List<Identifier>()
        };
        return declaration;
    }

    private Identifier ToIdentifier(JsonElement aliasNode)
    {
        var name = aliasNode.GetProperty("id").GetProperty("value").GetString()!;
        var start = aliasNode.GetProperty("span").GetProperty("start").GetUInt32();
        CodeLocation startLocation = _lnt.ToLocation(start);
        var end = aliasNode.GetProperty("span").GetProperty("end").GetUInt32();
        CodeLocation endLocation = _lnt.ToLocation(end);
        var identifier = new Identifier(name, _fileName, startLocation, endLocation);
        return identifier;
    }
}
