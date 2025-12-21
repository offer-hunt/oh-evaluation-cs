using System.Security.Claims;
using Oh.Evaluation.Api.Abstractions;

namespace Oh.Evaluation.Api.Services;


public class UserContextService(IHttpContextAccessor httpContextAccessor) : IUserContextService
{
    public Guid? GetCurrentUserId()
    {
        var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier) 
                          ?? httpContextAccessor.HttpContext?.User.FindFirst("sub");
            
        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }
        
        return null;
    }
}