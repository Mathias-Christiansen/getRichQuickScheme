namespace Contracts.Users;

public class CreateUserRequest
{
    public string Email { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Password { get; set; } = default!;
    
}