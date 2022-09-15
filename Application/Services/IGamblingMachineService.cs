using Domain.GamblingMachines;
using Domain.GamblingMachines.Abstractions;
using OneOf;
using OneOf.Types;

namespace Application.Services;

public interface IGamblingMachineService
{
    public OneOf<AbstractSlotMachine<TTileSet>, Error> GetSlotMachine<TTileSet>()
        where TTileSet : struct, Enum;
}