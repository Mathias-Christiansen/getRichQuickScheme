using Contracts.Errors.Interfaces;

namespace Contracts.Errors;

public record GamblingMachineNotFound() : INotFoundError
{
    public string? Message => "Could not find the gambling machine";
}