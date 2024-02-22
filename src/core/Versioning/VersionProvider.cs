namespace Hyperion.Core.Versioning;
public static class VersionProvider
{
    public static string Version { get; }
    static VersionProvider()
    {
        Version = typeof(VersionProvider).Assembly.GetName().Version?.ToString(4) ?? "0.0.0.0";
    }
}
