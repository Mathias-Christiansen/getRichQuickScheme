using Contracts.Common.TileSetsDto;

namespace Contracts.Common.SpinDto;

public class PlanetoidBaronessSpinDto : SpinResultsDto<PlanetoidBaronessTileSetDto>
{
    public List<PlanetoidBaronessStateDto> States { get; set; } = new();
}

public record PlanetoidBaronessStateDto(PlanetoidBaronessTileSetDto[][] Grid, float[] Multipliers);