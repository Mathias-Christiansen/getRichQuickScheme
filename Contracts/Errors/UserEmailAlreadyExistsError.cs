using Contracts.Errors.Interfaces;

namespace Contracts.Errors;

public record UserEmailAlreadyExistsError(string Email) : IAlreadyExistsError
{
    public string? Message => $"User Email '{Email}' Already Registered";
}