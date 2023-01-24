namespace SwcDotNet;

public interface IParserConfig
{
    public string Syntax { get; }
}

public abstract class CommonParserConfig : IParserConfig
{
    public bool? Comments { get; set; }
    public bool? Script { get; set; }
    public bool? IsModule { get; set; }
    public string? JscTarget { get; set; }
    public abstract string Syntax { get; }
}

public class TsParserConfig : CommonParserConfig
{
    public bool? Tsx { get; set; }
    public bool? Decorators { get; set; }
    public bool? DynamicImport { get; set; }
    public override string Syntax => "typescript";
}

public class EsParserConfig : CommonParserConfig
{
    public bool? Jsx { get; set; }
    public bool? FunctionBind { get; set; }
    public bool? Decorators { get; set; }
    public bool? DecoratorsBeforeExport { get; set; }
    public bool? ExportDefaultFrom { get; set; }
    public bool? ImportAssertions { get; set; }
    public override string Syntax => "ecmascript";
}
