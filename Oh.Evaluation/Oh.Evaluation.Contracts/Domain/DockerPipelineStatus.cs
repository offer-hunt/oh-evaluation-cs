namespace Contracts.Domain;

public enum DockerPipelineStatus
{
    Received,
    SyntaxError,
    CompileError,
    RunningTests,
    FailedTests,
    PassedTests,
    AiReviewPending,
    AiReviewed,
    Error
}