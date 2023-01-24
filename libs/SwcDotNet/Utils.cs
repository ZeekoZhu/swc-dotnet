using System.Text.Json;

namespace SwcDotNet;

public static class Utils
{
    public static string SerializeSwcConfig(IParserConfig config)
    {
        // in camelCase
        return JsonSerializer.Serialize(config, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }
}
