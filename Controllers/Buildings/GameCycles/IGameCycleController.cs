using UniverseRift.GameModelDatas.Players;

namespace UniverseRift.Controllers.Buildings.GameCycles
{
    public interface IGameCycleController
    {
        async Task<CycleEventsData> GetPlayerSave(int playerId) { return null; }
    }
}
