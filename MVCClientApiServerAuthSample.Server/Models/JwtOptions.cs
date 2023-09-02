namespace MVCClientApiServerAuthSample.Server.Models;

public class JwtOptions
{
    public const string SectionName = "Authentication:JWT";

    public required string Issuer { get; set; }

    public required string Audience { get; set; }

    public required string SecurityKey { get; set; }

    public bool IsValidateLifetime { get; set; }

    public TimeSpan Lifetime { get; set; }
}