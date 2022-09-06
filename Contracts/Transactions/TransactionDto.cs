namespace Contracts.Transactions;
public enum TransactionTypeDto
{
    Spin = 0,
    Transferal = 1,
    Bonus = 2
}

public class TransactionDto
{
    public Guid Id { get; init; } = default!;
    public decimal Amount { get; init; } = default!;
    public TransactionTypeDto Type { get; init; } = default!;
    public DateTime CreatedAt { get; init; } = default!;
}