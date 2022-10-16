using Domain.GamblingMachines.TileSets;

namespace Domain.GamblingMachines.PlanetoidBaroness;

public record PlanetoidBaronessState(PlanetoidBaronessTileSet[][] Grid, float[] Multipliers);