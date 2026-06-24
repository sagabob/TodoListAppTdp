using TodoListApi.Models;
using TodoListApi.Services;

namespace TodoListApi.Tests.Services;

public class AuthServiceTests
{
    private readonly AuthService _sut = new(
        TestConfiguration.Create(),
        new JwtTokenService(TestConfiguration.Create()),
        TestConfiguration.CreateLogger<AuthService>());

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsToken()
    {
        var request = new LoginRequest
        {
            Email = "demo@test.com",
            Password = "Password123!"
        };

        var result = await _sut.LoginAsync(request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("demo@test.com", result.Email);
        Assert.False(string.IsNullOrWhiteSpace(result.Token));
        Assert.True(result.ExpiresAt > DateTime.UtcNow);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ReturnsNull()
    {
        var request = new LoginRequest
        {
            Email = "demo@test.com",
            Password = "wrong-password"
        };

        var result = await _sut.LoginAsync(request, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidEmail_ReturnsNull()
    {
        var request = new LoginRequest
        {
            Email = "other@test.com",
            Password = "Password123!"
        };

        var result = await _sut.LoginAsync(request, CancellationToken.None);

        Assert.Null(result);
    }
}
