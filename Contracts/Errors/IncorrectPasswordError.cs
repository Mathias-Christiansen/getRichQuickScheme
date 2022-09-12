using Contracts.Errors.Interfaces;

namespace Contracts.Errors;

public record IncorrectPasswordError() : IValidationError
{
    public string? Message => "The password is incorrect";
}