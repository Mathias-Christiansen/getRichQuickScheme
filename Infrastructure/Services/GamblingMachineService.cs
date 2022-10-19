using Application.Services;
using Domain.GamblingMachines;
using Domain.GamblingMachines.Abstractions;
using OneOf;
using OneOf.Types;

namespace Infrastructure.Services;

public class GamblingMachineService : IGamblingMachineService
{

    private Dictionary<Type, ISlotMachine>? _slotMachines;

    private static Dictionary<Type, ISlotMachine> LoadSlotMachines()
    {
        return GamblingMachineProvider
            .GetSlotMachines()
            .ToDictionary(
                k => k.GetResultType(), 
                v => v
                );
    }

    public OneOf<AbstractSlotMachine<TResult>, Error> GetSlotMachine<TResult>() where TResult : IGamblingResult
    {
        _slotMachines ??= LoadSlotMachines(); //if _slotMachines is null load them
        if (_slotMachines.TryGetValue(typeof(TResult), out var slotMachine))
        {
            return slotMachine is AbstractSlotMachine<TResult> sm ? sm : new Error();
        }

        return new Error();


    }
}