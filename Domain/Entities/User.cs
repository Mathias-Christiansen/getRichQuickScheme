using Domain.Common;
using Domain.ValueObj;
using FluentValidation;
using OneOf;
using OneOf.Types;

namespace Domain.Entities;

public class User : Entity
{
    private List<Transaction> _transactions = new ();

    private User()
    {
    }

    public Guid Id { get; private set; } = default!;
    public EmailAddress Email { get; private set; } = default!;
    public Password Password { get; private set; } = default!;
    public string Name { get; private set; } = default!;

    public Secret? Secret { get; private set; }

    public IReadOnlyList<Transaction> Transactions
    {
        get => _transactions; 
        private set => _transactions = value.ToList();
    }

    public Money Balance { get; private set; } = default!;

    public static User Create(Guid id, EmailAddress email, Password password, string name)
    {
        var entity = new User()
        {
            Id = id,
            Email = email,
            Password = password,
            Name = name,
            Balance = Money.Create(0)
        };
        entity.Validate();
        return entity;
    }

    public Secret GenerateSecret()
    {
        var secret = Secret.Create();
        Secret = secret;
        return secret;
    }

    public OneOf<Transaction, Error> AddTransaction(Guid id, Money amount, TransactionType type)
    {
        if (Balance.SubUnit + amount.SubUnit < 0)
        {
            return new Error();
        }

        var transaction = Transaction.Create(id, amount, type);
        Balance = Balance.Combine(amount);
        _transactions.Add(transaction);
        return transaction;
    }
}

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).MaximumLength(128).NotEmpty();
        RuleFor(x => x.Email).SetValidator(new EmailAddressValidator()).NotNull();
        RuleFor(x => x.Password).SetValidator(new PasswordValidator()).NotNull();
        RuleFor(x => x.Secret).SetValidator(new SecretValidator()!).When(y => y.Secret is not null);
    }
}