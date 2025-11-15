namespace Auria.Structure.Configuration;

public class AppSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public JwtSettings Jwt { get; set; } = new();
    public CloudinarySettings Cloudinary { get; set; } = new();
    public GmailSettings Gmail { get; set; } = new();
    public SerilogSettings Serilog { get; set; } = new();
}

public class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpirationMinutes { get; set; } = 60;
}

public class CloudinarySettings
{
    public string CloudName { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string ApiSecret { get; set; } = string.Empty;
}

public class GmailSettings
{
    public string FromEmail { get; set; } = string.Empty;
    public string CredentialsPath { get; set; } = string.Empty;
    public string CredentialsJson { get; set; } = string.Empty;
}

public class SerilogSettings
{
    public string LogPath { get; set; } = "logs/auria-.log";
    public string MinimumLevel { get; set; } = "Information";
}
