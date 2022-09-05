using System.Text;
using Domain.Common;
using FluentValidation;

namespace Domain.ValueObj;

public class Secret : ValueObject
{
    private Secret()
    {
        
    }

    public string Bearer { get; private set; } = default!;
    public DateTime CreatedAt { get; private set; } = default!;

    public static Secret Create()
    {
        var valueObj = new Secret()
        {
            Bearer = GenerateBearerToken(),
            CreatedAt = DateTime.Now
        };
        valueObj.Validate();
        return valueObj;
    }
    
    private static string GenerateBearerToken()
    {
        const string chars = "abcdefghijklmnopqrstuvxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!#¤%&/()=_-";
        var random = new Random();
        var builder = new StringBuilder();
        foreach (var c in Enumerable.Range(0,4096).Select(_ => random.Next(0, chars.Length)).Select(x => chars[x]))
        {
            builder.Append(c);
        }

        return builder.ToString();
    }

    public bool Compare(string bearer)
    {
        if (DateTime.Now.Subtract(CreatedAt) > TimeSpan.FromHours(1))
            return false;
        return bearer == Bearer;
    }

    protected override IEnumerable<object?> GetFields()
    {
        yield return Bearer;
        yield return CreatedAt;
    }
}

public class SecretValidator : AbstractValidator<Secret>
{
    public SecretValidator()
    {
        RuleFor(x => x.Bearer).Length(4096).NotEmpty();
        RuleFor(x => x.CreatedAt).LessThanOrEqualTo(_ => DateTime.Now);
    }
}