using Contracts.Errors.Interfaces;

namespace Contracts.Errors;

public record UserNotFound() : INotFoundError
{
    public string? Message => "User could not be found";
}