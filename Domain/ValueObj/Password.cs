using System.Security.Cryptography;
using System.Text;
using Domain.Common;
using FluentValidation;

namespace Domain.ValueObj;

public class Password : ValueObject
{
    private Password()
    {
    }

    public string Salt { get; private set; } = default!;
    public string Hashed { get; private set; } = default!;

    public static Password Create(string password)
    {
        var salt = GenerateSalt();
        var valueObj = new Password()
        {
            Salt = salt,
            Hashed = HashPassword(salt, password)
        };
        valueObj.Validate();
        return valueObj;
    }

    private static string HashPassword(string salt, string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password + salt));
        return Encoding.UTF8.GetString(bytes);
    }

    private static string GenerateSalt()
    {
        const string chars = "abcdefghijklmnopqrstuvxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!#¤%&/()=_-";
        var random = new Random();
        var builder = new StringBuilder();
        foreach (var c in Enumerable.Range(0,32).Select(_ => random.Next(0, chars.Length)).Select(x => chars[x]))
        {
            builder.Append(c);
        }

        return builder.ToString();
    }

    public bool Compare(Password? pas1)
    {
        if (pas1 is null)
            return false;
        return Hashed == pas1.Hashed;
    }
    
    public bool Compare(string? pas1)
    {
        if (pas1 is null)
            return false;
        return Hashed == HashPassword(Salt, pas1);
    }

    protected override IEnumerable<object?> GetFields()
    {
        yield return Hashed;
        yield return Salt;
    }
}

public class PasswordValidator : AbstractValidator<Password>
{
    public PasswordValidator()
    {
        RuleFor(x => x.Hashed).MaximumLength(4096).NotEmpty();
        RuleFor(x => x.Salt).Length(32).NotEmpty();
    }
}