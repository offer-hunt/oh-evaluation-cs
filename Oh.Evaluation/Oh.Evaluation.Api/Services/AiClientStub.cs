using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Contracts.Internal.Ai;
using Microsoft.Extensions.Options;
using Oh.Evaluation.Api.ApiClients;
using Oh.Evaluation.Api.Config;

namespace Oh.Evaluation.Api.Services;

public class AiClientStub : IAiClient
{
    private readonly HttpClient _http;
    private readonly OpenRouterSettings _options;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    
    public Task<CodeReviewResponse> GetCodeReview(CodeReviewRequest request, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public AiClientStub(HttpClient httpClient, IOptions<OpenRouterSettings> options)
    {
        _options = options.Value;
        _http = httpClient;

        if (string.IsNullOrWhiteSpace(_options.ApiKey))
            throw new InvalidOperationException("OpenRouter ApiKey is not configured");

        _http.BaseAddress = new Uri(_options.BaseUrl.TrimEnd('/') + "/");
        _http.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);

        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _options.ApiKey);

        _http.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

        _http.DefaultRequestHeaders.Add("HTTP-Referer", _options.Referer);
        _http.DefaultRequestHeaders.Add("Referer", _options.Referer);
    }

    public async Task<TextReviewResponse> GetTextReview(
        TextReviewRequest request,
        CancellationToken ct = default)
    {
        var messages = BuildMessages(request);

        var payload = new
        {
            model = _options.Model,
            messages,
            temperature = 0.2
        };

        using var content = new StringContent(
            JsonSerializer.Serialize(payload, JsonOptions),
            Encoding.UTF8,
            "application/json");

        using var response = await _http.PostAsync("chat/completions", content, ct);

        var body = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException(
                $"OpenRouter error {(int)response.StatusCode}: {body}");

        var completion = JsonDocument.Parse(body);
        var text = ExtractAssistantText(completion);

        if (string.IsNullOrWhiteSpace(text))
            throw new InvalidOperationException("Empty LLM response");

        return ParseReview(text);
    }

    private static object[] BuildMessages(TextReviewRequest r) =>
    [
        new
        {
            role = "system",
            content =
                """
                You are an expert examiner.
                Evaluate the user's answer strictly according to the rubric.
                Respond ONLY in valid JSON with this schema:

                {
                  "score": number (0-100),
                  "summary": string,
                  "suggestions": string (semicolon separated)
                }
                """
        },
        new
        {
            role = "user",
            content =
                $"""
                Question:
                {r.Question}

                Rubric (correct answer):
                {r.Rubric}

                User answer:
                {r.Answer}
                """
        }
    ];

    private static string ExtractAssistantText(JsonDocument doc)
    {
        var root = doc.RootElement;

        if (!root.TryGetProperty("choices", out var choices) ||
            choices.GetArrayLength() == 0)
            return string.Empty;

        var message = choices[0].GetProperty("message");

        return message.GetProperty("content").GetString() ?? string.Empty;
    }
    
    private static string CleanJson(string text)
    {
        text = text.Trim();
    
        // Удаляем все возможные префиксы
        string[] prefixes = { "```json", "```", "json" };
    
        foreach (var prefix in prefixes)
        {
            if (text.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                text = text[prefix.Length..].TrimStart();
            
                // Если после префикса идет перевод строки, пропускаем его
                if (text.StartsWith('\n'))
                    text = text[1..];
            
                break;
            }
        }
    
        // Удаляем суффиксы
        if (text.EndsWith("```"))
            text = text[..^3].TrimEnd();
    
        return text.Trim();
    }


    private static TextReviewResponse ParseReview(string json)
    {
        try
        {
            var cleanJson = CleanJson(json);

            return JsonSerializer.Deserialize<TextReviewResponse>(cleanJson, JsonOptions)
                   ?? throw new InvalidOperationException("Failed to deserialize review");
        }
        catch (JsonException e)
        {
            throw new InvalidOperationException(
                $"Invalid review JSON from LLM: {json}", e);
        }
    }
}