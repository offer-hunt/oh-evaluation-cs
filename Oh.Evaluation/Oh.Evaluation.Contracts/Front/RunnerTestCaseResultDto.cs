using Contracts.Domain;

namespace Contracts.Front;

public record RunnerTestCaseResultDto
{
    public Guid Id { get; set; }
    public Guid? TestCaseId { get; set; }
    public TestCaseStatus Status { get; set; }
    public int? TimeMs { get; set; }
    public int? MemoryKb { get; set; }
        
    public string? StderrSnippet { get; set; }
        
    public string? DiffSnippet { get; set; }
}