using Contracts.Errors.Interfaces;

namespace Contracts.Errors;

public record InsufficientFundsError(decimal Amount, decimal Balance) : IValidationError
{
    public string? Message => $"u hab no money u tried using {Amount}dkk but u only hab {Balance}dkk, stupiiid";
}