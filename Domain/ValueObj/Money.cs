using Domain.Common;
using FluentValidation;

namespace Domain.ValueObj;

public class Money : ValueObject
{

    public long SubUnit { get; private set; } = default!;
    
    private Money()
    {
        
    }

    public static Money Create(decimal amount)
    {
        var valueObj = new Money()
        {
            SubUnit = (long)(decimal.Round(amount, 2) * 100),
        };
        valueObj.Validate();
        return valueObj;
    }

    public decimal GetUnits()
    {
        return decimal.Round(Convert.ToDecimal(SubUnit) / 100, 2);
    }
    
    protected override IEnumerable<object?> GetFields()
    {
        yield return SubUnit;
    }

    public override string ToString()
    {
        return GetUnits() + " dkk";
    }

    public void Combine(Money other)
    {
        SubUnit += other.SubUnit;
    }
}

public class MoneyValidator : AbstractValidator<Money>
{
    public MoneyValidator()
    {
        RuleFor(x => x.SubUnit).InclusiveBetween(-10000, 10000);
    }
}