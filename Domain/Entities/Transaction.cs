using Domain.Common;
using Domain.ValueObj;
using FluentValidation;

namespace Domain.Entities;

public enum TransactionType
{
    Spin = 0,
    Transferal = 1,
    Bonus = 2
}

public class Transaction : Entity
{
    public Guid Id { get; private set; } = default!;
    public Money Amount { get; private set; } = default!;
    public TransactionType Type { get; private set; } = default!;
    public DateTime CreatedAt { get; private set; } = default!;
    
    
    private Transaction()
    {
        
    }

    public static Transaction Create(Guid id, Money amount, TransactionType type)
    {
        var entity = new Transaction()
        {
            Id = id,
            Amount = amount,
            Type = type,
            CreatedAt = DateTime.Now
        };
        entity.Validate();
        return entity;
    }
}

public class TransactionValidator : AbstractValidator<Transaction>
{
    public TransactionValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Amount).SetValidator(new MoneyValidator()).NotNull();
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.CreatedAt).LessThanOrEqualTo(_ => DateTime.Now);
    }
}