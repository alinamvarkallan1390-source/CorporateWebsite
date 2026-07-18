using CorporateWebsite.Web.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace CorporateWebsite.Web.Services;

public interface ICurrentUserService
{
    int? UserId { get; }
    string? UserName { get; }
    string? Email { get; }
    string? FirstName { get; }
    string? LastName { get; }
    IEnumerable<string> Roles { get; }
    bool IsAuthenticated { get; }
    bool IsInRole(string role);
    string? GetClaim(string claimType);
}

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? UserId
    {
        get
        {
            var claim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out var id) ? id : null;
        }
    }

    public string? UserName => _httpContextAccessor.HttpContext?.User?.Identity?.Name;

    public string? Email => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;

    public string? FirstName => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.GivenName)?.Value;

    public string? LastName => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Surname)?.Value;

    public IEnumerable<string> Roles => _httpContextAccessor.HttpContext?.User?.FindAll(ClaimTypes.Role)?.Select(c => c.Value) ?? Enumerable.Empty<string>();

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public bool IsInRole(string role) => _httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false;

    public string? GetClaim(string claimType) => _httpContextAccessor.HttpContext?.User?.FindFirst(claimType)?.Value;
}