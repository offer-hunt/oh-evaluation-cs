using Contracts.Internal.Course;
using Refit;

namespace Oh.Evaluation.Api.ApiClients;

public interface IPagesClient
{
    [Get("/{pageId}/code-task")]
    public Task<CodeTaskDto> GetCodeTaskTestCases(Guid id);
}