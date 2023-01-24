namespace SwcDotNet;

public interface IParserConfig
{
    public string Syntax { get; }
}

public class TsParserConfig : IParserConfig
{
    public bool? Tsx { get; set; }
    public bool? Decorators { get; set; }
    public bool? DynamicImport { get; set; }
    public string Syntax { get; } = "typescript";
}

public class EsParserConfig : IParserConfig
{
    public bool? Jsx { get; set; }
    public bool? FunctionBind { get; set; }
    public bool? Decorators { get; set; }
    public bool? DecoratorsBeforeExport { get; set; }
    public bool? ExportDefaultFrom { get; set; }
    public bool? ImportAssertions { get; set; }
    public string Syntax { get; set; } = "ecmascript";
}
