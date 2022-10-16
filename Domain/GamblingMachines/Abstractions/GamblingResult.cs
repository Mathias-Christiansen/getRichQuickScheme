namespace Domain.GamblingMachines.Abstractions;

public abstract class GamblingResult
{
    public IReadOnlyCollection<float> Multipliers { get; private set; }


    public GamblingResult(IReadOnlyCollection<float> multipliers)
    {
        Multipliers = multipliers;
    }
    
    public float TotalMultiplier()
    {
        return Multipliers.Sum();
    }
}

public interface IGamblingResult
{
    public float TotalMultiplier();
}