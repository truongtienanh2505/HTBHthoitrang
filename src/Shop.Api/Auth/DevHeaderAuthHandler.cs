using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Shop.Api.Auth;

/// <summary>
/// Auth "tạm" cho DEV để test Authorization/Role.
/// Chỉ hoạt động khi Environment = Development.
/// </summary>
public sealed class DevHeaderAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IWebHostEnvironment _env;

    public DevHeaderAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IWebHostEnvironment env)
        : base(options, logger, encoder)
    {
        _env = env;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!_env.IsDevelopment())
            return Task.FromResult(AuthenticateResult.Fail("DevHeader auth is disabled outside Development."));

        if (!Request.Headers.TryGetValue(DevHeaderAuthDefaults.HeaderUserId, out var userIdRaw))
            return Task.FromResult(AuthenticateResult.NoResult());

        if (!int.TryParse(userIdRaw.ToString(), out var userId) || userId <= 0)
            return Task.FromResult(AuthenticateResult.Fail("Invalid X-Dev-UserId."));

        Request.Headers.TryGetValue(DevHeaderAuthDefaults.HeaderRole, out var roleRaw);
        var role = string.IsNullOrWhiteSpace(roleRaw.ToString()) ? "User" : roleRaw.ToString().Trim();

        var name = Request.Headers.TryGetValue(DevHeaderAuthDefaults.HeaderName, out var nameRaw)
            ? nameRaw.ToString().Trim()
            : $"dev-user-{userId}";

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new("sub", userId.ToString()),
            new(ClaimTypes.Name, name),
            new(ClaimTypes.Role, role)
        };

        var identity = new ClaimsIdentity(claims, DevHeaderAuthDefaults.Scheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, DevHeaderAuthDefaults.Scheme);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}