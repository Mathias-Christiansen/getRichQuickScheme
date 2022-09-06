namespace Contracts.Errors;

public record NegativeAmountTransferError(decimal Amount) : IError
{
    public string? Message => $"Cannot transfer negative amount '{Amount}' into account balance";
}