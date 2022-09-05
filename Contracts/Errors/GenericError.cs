namespace Contracts.Errors;

public record GenericError(string ErrorMessage) : IError
{
    public string? Message => ErrorMessage;
}