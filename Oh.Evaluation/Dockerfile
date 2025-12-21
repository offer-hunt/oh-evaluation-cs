# ===== BUILD STAGE =====
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Oh.Evaluation.sln .
COPY Oh.Evaluation.Api/Oh.Evaluation.Api.csproj Oh.Evaluation.Api/
COPY Oh.Evaluation.Core/Oh.Evaluation.Core.csproj Oh.Evaluation.Core/
COPY Oh.Evaluation.Contracts/Oh.Evaluation.Contracts.csproj Oh.Evaluation.Contracts/
COPY Oh.Evaluation.Infrastructure/Oh.Evaluation.Infrastructure.csproj Oh.Evaluation.Infrastructure/
COPY Oh.Evaluation.Tests/Oh.Evaluation.Tests.csproj Oh.Evaluation.Tests/

RUN dotnet restore

COPY . .

RUN dotnet publish Oh.Evaluation.Api/Oh.Evaluation.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ===== RUNTIME STAGE =====
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Oh.Evaluation.Api.dll"]
