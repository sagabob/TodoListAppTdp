namespace TodoListApi.Models;

public class AuthSessionResponse
{
    public required string Email { get; set; }
    public DateTime ExpiresAt { get; set; }
}
