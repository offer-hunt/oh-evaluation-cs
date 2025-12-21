namespace Oh.Evaluation.Api.Abstractions;

public interface IUserContextService
{
    Guid? GetCurrentUserId();
}
