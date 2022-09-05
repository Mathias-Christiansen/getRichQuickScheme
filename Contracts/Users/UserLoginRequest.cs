namespace Contracts.Users;

public class UserLoginRequest
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
}