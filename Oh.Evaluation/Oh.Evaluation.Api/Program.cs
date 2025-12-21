using Contracts.Front;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Oh.Evaluation.Api.Abstractions;
using Oh.Evaluation.Api.ApiClients;
using Oh.Evaluation.Api.config;
using Oh.Evaluation.Api.Config;
using Oh.Evaluation.Api.Services;
using Oh.Evaluation.Infrastructure;
using Refit;

var builder = WebApplication.CreateBuilder(args);

// Конфигурация
builder.Configuration
    .AddJsonFile("Properties/appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Настройки
builder.Services.Configure<AuthSettings>(
    builder.Configuration.GetSection("AuthSettings"));
builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("DatabaseSettings"));
builder.Services.Configure<ApiSettings>(
    builder.Configuration.GetSection("ApiSettings"));

// Привязываем настройки к переменным
var authSettings = builder.Configuration.GetSection("AuthSettings").Get<AuthSettings>() ?? new AuthSettings();
var dbSettings = builder.Configuration.GetSection("DatabaseSettings").Get<DatabaseSettings>() ?? new DatabaseSettings();
var apiSettings = builder.Configuration.GetSection("ApiSettings").Get<ApiSettings>() ?? new ApiSettings();

// Настройка DbContext
builder.Services.AddDbContext<EvaluationDbContext>(options =>
{
    var connectionString = $"Host={dbSettings.Host};Port={dbSettings.Port};Database={dbSettings.Name};Username={dbSettings.User};Password={dbSettings.Password}";
    options.UseNpgsql(connectionString);
});

// JWT аутентификация
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // для локальной разработки

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = authSettings.AuthIssuer,

            ValidateAudience = true,
            ValidAudience = authSettings.AuthAudience,

            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,

            IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
            {
                using var httpClient = new HttpClient();
                var jwksJson = httpClient.GetStringAsync(authSettings.AuthJwksUrl).GetAwaiter().GetResult();
                var jwks = new JsonWebKeySet(jwksJson);

                return jwks.Keys.Where(k => k.Kid == kid);
            }
        };
    });


builder.Services.AddAuthorization();

builder.Services.AddHttpClient();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ICodeEvaluationService, CodeEvaluationService>();
builder.Services.AddScoped<IQuizEvaluationService, QuizEvaluationService>();
builder.Services.AddScoped<ISubmissionService, StudentSubmissionService>();
builder.Services.AddScoped<ITextEvaluationService, TextEvaluationService>();
builder.Services.AddScoped<IUserContextService, UserContextService>();

builder.Services.AddRefitClient<IAiClient>().ConfigureHttpClient(c => c.BaseAddress = new Uri(apiSettings.AiClientBase));
builder.Services.AddRefitClient<ICourseClient>().ConfigureHttpClient(c => c.BaseAddress = new Uri(apiSettings.CourseClientBase));
builder.Services.AddRefitClient<ILearningClient>().ConfigureHttpClient(c => c.BaseAddress = new Uri(apiSettings.LearningClientBase));
builder.Services.AddRefitClient<IPagesClient>().ConfigureHttpClient(c => c.BaseAddress = new Uri(apiSettings.PageClientBase));


// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference 
                { 
                    Type = ReferenceType.SecurityScheme, 
                    Id = "Bearer" 
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}


//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/api/v1/submissions/{submissionId}/result",
    ([FromQuery] Guid submissionId, [FromServices] ISubmissionService service) => service.GetResult(submissionId))
    .RequireAuthorization();

app.MapPost("/api/v1/submissions",
    ([FromBody] StudentSubmissionRequest request, [FromServices] ISubmissionService service) => service.SubmitTask(request))
    .RequireAuthorization();

app.MapGet("/ping",  () => "pong");

app.MapGet("/secure-ping",  () => "secure pong").RequireAuthorization();

app.Run();