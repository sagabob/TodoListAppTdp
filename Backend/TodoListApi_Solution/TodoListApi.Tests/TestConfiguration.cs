using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace TodoListApi.Tests;

internal static class TestConfiguration
{
    public static IConfiguration Create() =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["DemoAuth:Email"] = "demo@test.com",
                ["DemoAuth:Password"] = "Password123!",
                ["Jwt:Key"] = "TodoListDemo-SuperSecret-Key-AtLeast-32-Chars!",
                ["Jwt:Issuer"] = "TodoListApi",
                ["Jwt:Audience"] = "TodoWeb",
                ["Jwt:ExpiresMinutes"] = "60"
            })
            .Build();

    public static ILogger<T> CreateLogger<T>() => Mock.Of<ILogger<T>>();
}
