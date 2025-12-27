namespace Oh.Evaluation.Api.Config;

public sealed class OpenRouterSettings
{
    public required string ApiKey { get; init; }
    public string BaseUrl { get; init; } = "https://openrouter.ai/api/v1";
    public string Model { get; init; } = "z-ai/glm-4.5-air:free";
    public string Referer { get; init; } = "http://localhost";
    public int TimeoutSeconds { get; init; } = 60;
}
