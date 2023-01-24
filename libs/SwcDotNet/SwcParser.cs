namespace SwcDotNet;

public class SwcParser
{
    public SwcParser(IParserConfig config)
    {
        Config = config;
    }

    public IParserConfig Config { get; set; }

    public string Parse(string source, string? filename = null)
    {
        var options = Utils.SerializeSwcConfig(Config);
        var parseParams = new ParseParams
        {
            src = source,
            options = options,
            filename = OptionStringRef.FromNullable(filename is null ? null : new StringRef { value = filename })
        };
        using var swcWrap = SwcWrap.New(parseParams);
        return swcWrap.Parse();
    }
}
