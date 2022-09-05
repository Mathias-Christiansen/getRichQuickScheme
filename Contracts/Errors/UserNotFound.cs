namespace Contracts.Errors;

public record UserNotFound() : IError
{
    public string? Message => "User could not be found";
}