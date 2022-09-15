namespace Domain.GamblingMachines.Abstractions;

public static class GamblingMachineProvider
{
    public static IEnumerable<ISlotMachine> GetSlotMachines()
    {
        return typeof(ISlotMachine).Assembly
            .GetExportedTypes()
            .Where(x => x.IsAssignableTo(typeof(ISlotMachine))
                        && !x.IsAbstract && x.IsClass)
            .Select(x => Activator.CreateInstance(x) as ISlotMachine)
            .Where(x => x is not null)
            .Select(x => x!)
            .ToList();
    }
}