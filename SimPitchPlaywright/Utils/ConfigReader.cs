using System.Text.Json.Nodes;

namespace SimPitchPlaywright.Utils;

public static class ConfigReader
{
    private static readonly Lazy<JsonNode> Config = new(() =>
        JsonNode.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "appsettings.json")))
        ?? throw new InvalidOperationException("appsettings.json is empty."));

    public static string GetBaseUrl() => Get("baseUrl");
    public static string GetApiBaseUrl() => Get("apiBaseUrl");
    public static int GetDefaultTimeoutSec() => int.Parse(Get("timeouts.default"));
    public static int GetNavigationTimeoutSec() => int.Parse(Get("timeouts.navigation"));

    /// <summary>off | on | retain-on-failure</summary>
    public static string GetTracingMode() => Get("tracing");

    /// <summary>
    /// Environment variables take precedence, e.g. "timeouts.default" -> TIMEOUTS_DEFAULT.
    /// </summary>
    public static string Get(string key)
    {
        var envValue = Environment.GetEnvironmentVariable(key.Replace(".", "_").ToUpperInvariant());
        if (!string.IsNullOrEmpty(envValue))
        {
            return envValue;
        }

        JsonNode? node = Config.Value;
        foreach (var part in key.Split('.'))
        {
            node = node?[part];
        }
        return node?.ToString() ?? string.Empty;
    }
}
