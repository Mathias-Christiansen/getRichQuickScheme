using System.Reflection;
using FluentValidation;

namespace Domain.Common;

public abstract class Entity : IValidatable
{
    public void Validate() => ValidationService.Validate(this);
}