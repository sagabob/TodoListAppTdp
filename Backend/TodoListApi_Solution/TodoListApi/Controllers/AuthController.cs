using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApi.Auth;
using TodoListApi.Models;
using TodoListApi.Services;

namespace TodoListApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(
    IAuthService authService,
    IWebHostEnvironment environment) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<AuthSessionResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var response = await authService.LoginAsync(request, cancellationToken);
        if (response is null)
            return Unauthorized();

        Response.Cookies.Append(
            AuthCookieConstants.AccessToken,
            response.Token,
            CreateCookieOptions(response.ExpiresAt));

        return Ok(new AuthSessionResponse
        {
            Email = response.Email,
            ExpiresAt = response.ExpiresAt
        });
    }

    [Authorize]
    [HttpGet("me")]
    public ActionResult<AuthSessionResponse> Me()
    {
        var email = User.FindFirstValue(ClaimTypes.Email)!;

        return Ok(new AuthSessionResponse
        {
            Email = email,
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete(AuthCookieConstants.AccessToken, new CookieOptions { Path = "/" });
        return NoContent();
    }

    private CookieOptions CreateCookieOptions(DateTime expiresAt) => new()
    {
        HttpOnly = true,
        Secure = !environment.IsDevelopment(),
        SameSite = SameSiteMode.Lax,
        Expires = expiresAt,
        Path = "/"
    };
}
