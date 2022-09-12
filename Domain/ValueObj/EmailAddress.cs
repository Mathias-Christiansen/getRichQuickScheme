using Domain.Common;
using FluentValidation;

namespace Domain.ValueObj;

public class EmailAddress : ValueObject
{
    private EmailAddress()
    {
        
    }

    public string Email { get; private set; } = default!;

    public static EmailAddress Create(string email)
    {
        var valueObj = new EmailAddress()
        {
            Email = email
        };
        valueObj.Validate();
        return valueObj;
    }

    protected override IEnumerable<object?> GetFields()
    {
        yield return Email;
    }
}

public class EmailAddressValidator : AbstractValidator<EmailAddress>
{
    public EmailAddressValidator()
    {
        RuleFor(x => x.Email).EmailAddress();
    }
}