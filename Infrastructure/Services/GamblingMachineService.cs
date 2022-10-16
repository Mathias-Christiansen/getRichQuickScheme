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

    public OneOf<AbstractSlotMachine<TTileSet>, Error> GetSlotMachine<TTileSet>() where TTileSet : struct, Enum
    {
        _slotMachines ??= LoadSlotMachines(); //if _slotMachines is null load them
        if (_slotMachines.TryGetValue(typeof(TTileSet), out var slotMachine))
        {
            return slotMachine is AbstractSlotMachine<TTileSet> sm ? sm : new Error();
        }

        return new Error();


    }
}