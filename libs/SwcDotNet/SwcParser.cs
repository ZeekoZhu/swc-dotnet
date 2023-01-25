namespace SwcDotNet;

public class SwcParser : IDisposable
{
    private readonly SwcWrap _swcWrap;

    /**
     * see https://swc.rs/docs/usage/core#parse
     */
    public SwcParser(IParserConfig config)
    {
        _swcWrap = SwcWrap.New();
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
        return _swcWrap.Parse(parseParams);
    }

    /// <summary>
    /// Convert ast span position back to line and column
    /// </summary>
    /// <param name="offset"></param>
    /// <returns>
    /// line, 1-based line number
    /// <br/>
    /// col, 0-based column number
    /// </returns>
    public (ulong line, ulong col) GetPosition(uint offset)
    {
        var pos = _swcWrap.LookupChar(offset);
        return (pos.line, pos.col);
    }

    public void Dispose()
    {
        _swcWrap.Dispose();
    }
}
