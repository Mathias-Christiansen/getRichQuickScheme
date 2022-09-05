using System.Reflection;
using FluentValidation;

namespace Domain.Common;

public static class ValidationService
{
    private static List<IValidator>? _validators;

    private static List<IValidator> Validators => _validators ??= Assembly.GetExecutingAssembly()
        .GetExportedTypes()
        .Where(x => x.IsAssignableTo(typeof(IValidator)))
        .Select(y => Activator.CreateInstance(y) as IValidator)
        .Where(x => x is not null)
        .ToList()!;
    
    public static void Validate(object instance)
    {
        var mytype = instance.GetType();
        var context = new ValidationContext<object>(instance);
        var errors = Validators
            .Where(x => x.GetType().GetInterfaces().Any(z => z.GenericTypeArguments.Any(y => y == mytype)))
            .Select(x => x.Validate(context))
            .SelectMany(x => x.Errors)
            .Where(x => x is not null)
            .ToList();
        if (errors.Any())
        {
            throw new ValidationException(errors);
        }
    }
}