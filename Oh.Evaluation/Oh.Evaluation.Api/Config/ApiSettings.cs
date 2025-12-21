namespace Oh.Evaluation.Api.Config;

public class ApiSettings
{
    public string AiClientBase { get; set; } = "http://localhost:8081";
    public string CourseClientBase { get; set; } = "http://localhost:8080/api/course";
    public string LearningClientBase { get; set; } = "http://localhost:8080";
    public string PageClientBase { get; set; } = "http://localhost:8080/api/pages";
}