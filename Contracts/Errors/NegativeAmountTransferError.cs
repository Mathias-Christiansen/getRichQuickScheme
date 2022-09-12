using Contracts.Errors.Interfaces;

namespace Contracts.Errors;

public record NegativeAmountTransferError(decimal Amount) : IValidationError
{
    public string? Message => $"Cannot transfer negative amount '{Amount}' into account balance";
}