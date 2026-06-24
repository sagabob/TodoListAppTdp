using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TodoListApi.Services;

namespace TodoListApi.Tests.Services;

public class JwtTokenServiceTests
{
    private readonly IConfiguration _configuration = TestConfiguration.Create();
    private readonly JwtTokenService _sut;

    public JwtTokenServiceTests()
    {
        _sut = new JwtTokenService(_configuration);
    }

    [Theory]
    [InlineData("demo@test.com")]
    [InlineData("other@test.com")]
    [InlineData("bob@email.com")]
    public void CreateToken_ReturnsValidJwtWithEmailClaim(string email)
    {
        var response = _sut.CreateToken(email);

        Assert.Equal(email, response.Email);
        Assert.False(string.IsNullOrWhiteSpace(response.Token));
        Assert.True(response.ExpiresAt > DateTime.UtcNow);

        var handler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _configuration["Jwt:Issuer"],
            ValidAudience = _configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!))
        };

        var principal = handler.ValidateToken(response.Token, validationParameters, out _);
        var emailClaim = principal.FindFirstValue(ClaimTypes.Email);

        Assert.Equal(email, emailClaim);
    }
}
