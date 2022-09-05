namespace Contracts.Errors;

public record UserEmailAlreadyExistsError(string Email) : IError
{
    public string? Message => $"User Email '{Email}' Already Registered";
}