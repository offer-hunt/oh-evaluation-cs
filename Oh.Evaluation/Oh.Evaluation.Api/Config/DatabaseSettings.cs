namespace Oh.Evaluation.Api.Config;

public class DatabaseSettings
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5432;
    public string User { get; set; } = "evaluation_user";
    public string Password { get; set; } = "evaluation_password";
    public string Name { get; set; } = "evaluation_db";
    public string Schema { get; set; } = "evaluation";
}