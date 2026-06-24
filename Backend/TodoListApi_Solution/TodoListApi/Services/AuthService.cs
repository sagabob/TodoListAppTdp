using TodoListApi.Models;

namespace TodoListApi.Services;

public class AuthService(
    IConfiguration configuration,
    JwtTokenService jwtTokenService,
    ILogger<AuthService> logger) : IAuthService
{
    public Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var demoEmail = configuration["DemoAuth:Email"];
        var demoPassword = configuration["DemoAuth:Password"];

        if (!string.Equals(request.Email, demoEmail, StringComparison.OrdinalIgnoreCase) ||
            request.Password != demoPassword)
        {
            logger.LogWarning("Login failed for {Email}", request.Email);
            return Task.FromResult<AuthResponse?>(null);
        }

        logger.LogInformation("User {Email} logged in", request.Email);
        return Task.FromResult<AuthResponse?>(jwtTokenService.CreateToken(request.Email));
    }
}
