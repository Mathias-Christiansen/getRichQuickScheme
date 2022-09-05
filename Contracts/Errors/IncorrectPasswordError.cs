namespace Contracts.Errors;

public record IncorrectPasswordError() : IError
{
    public string? Message => "The password is incorrect";
}