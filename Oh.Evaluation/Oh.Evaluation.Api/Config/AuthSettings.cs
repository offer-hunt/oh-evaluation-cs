namespace Oh.Evaluation.Api.config;

public class AuthSettings
{
    public string AuthIssuer { get; set; } = "http://localhost:8080";
    public string AuthJwksUrl { get; set; } = "http://localhost:8080/oauth2/jwks";
    public string AuthAudience { get; set; } = "offerhunt-api";
    public string ServerPort { get; set; } = "8080";
}