namespace WebApplication1.Core;
public static class AppEnvironment
{
    public static string GetName()
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        return env switch
        {
            "Production" => "prod",
            "Uat" => "uat",
            _ => "dev"
        };
    }

    public static double CacheExpired => double.Parse(Environment.GetEnvironmentVariable("CACHE_EXPIRED") ?? "10");

    public static string DateFormat => Environment.GetEnvironmentVariable("DATE_FORMAT") ?? "dd/MM/yyyy";

    public static string DbConnection => Environment.GetEnvironmentVariable("DB_CONNECTION") ??
                                            "Server=wickedly-sheltering-chipmunk.data-1.apse1.tembo.io;Port:5432;Database=test_db;User Id=postgres;Password=cPnw6LZ4smh3POBf;";

    public static string JwtIssuer => Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "https://dev.eventhub.one";

    public static string JwtAudience =>
        Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "https://dev.eventhub.one";

    public static string JwtKey => Environment.GetEnvironmentVariable("JWT_KEY") ??
                                   "ra8FXsc1Xv6FjN8cuxMDYcKeP4aQ4XRmKZyGnyhLRhuJ";

    public static double JwtExpired => double.Parse(Environment.GetEnvironmentVariable("JWT_EXPIRED") ?? "3600");

    public static double JwtRefreshExpired =>
        double.Parse(Environment.GetEnvironmentVariable("JWT_REFRESH_EXPIRED") ?? "2592000");

}