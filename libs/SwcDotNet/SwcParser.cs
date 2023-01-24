namespace SwcDotNet;

public class SwcParser
{
    /**
     * see https://swc.rs/docs/usage/core#parse
     */
    public SwcParser(IParserConfig config)
    {
        SwcOptions = Utils.SerializeSwcConfig(config);
    }

    /**
     * see https://swc.rs/docs/usage/core#parse
     */
    public SwcParser(dynamic swcConfig)
    {
        SwcOptions = Utils.SerializeSwcConfig(swcConfig);
    }

    private string SwcOptions { get; set; }

    public string Parse(string source, string? filename = null)
    {
        var parseParams = new ParseParams
        {
            src = source,
            options = SwcOptions,
            filename = OptionStringRef.FromNullable(filename is null ? null : new StringRef { value = filename })
        };
        using var swcWrap = SwcWrap.New(parseParams);
        return swcWrap.Parse();
    }
}
