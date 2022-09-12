namespace Contracts.Errors.Interfaces;

public record GenericError(string ErrorMessage) : IError
{
    public string? Message => ErrorMessage;
}