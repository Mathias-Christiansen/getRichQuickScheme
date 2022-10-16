namespace Domain.GamblingMachines.Abstractions;

public abstract class AbstractSlotMachine<TResult> : ISlotMachine
    where TResult : IGamblingResult
{
    
    public abstract TResult Spin();


    public Type GetResultType()
    {
        return typeof(TResult);
    }
}