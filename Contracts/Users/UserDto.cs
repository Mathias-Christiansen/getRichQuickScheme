namespace Contracts.Users;

public class UserDto
{
    public Guid Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
}