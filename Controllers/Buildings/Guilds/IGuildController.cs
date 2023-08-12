using UniverseRift.GameModelDatas.Cities;

namespace UniverseRift.Controllers.Buildings.Guilds
{
    public interface IGuildController
    {
        async Task<BuildingWithFightTeamsData> GetPlayerSave(int playerId) { return null; }
    }
}
