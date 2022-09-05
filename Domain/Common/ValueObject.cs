using System.Collections;

namespace Domain.Common;

public abstract class ValueObject : IValidatable
{
    public void Validate() => ValidationService.Validate(this);

    protected abstract IEnumerable<object?> GetFields();

    public override bool Equals(object? obj)
    {
        if (obj is ValueObject vo) return vo.GetFields().SequenceEqual(GetFields());
        return false;
    }

    public override int GetHashCode()
    {
        return GetFields()
            .Select(x => x is null ? 0 : x.GetHashCode())
            .Aggregate(HashCode.Combine);
    }
}