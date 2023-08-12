using UniverseRift.GameModelDatas.Cities;

namespace UniverseRift.Controllers.Buildings.Tutorials
{
    public interface ITutorialController
    {
        async Task<SimpleBuildingData> GetPlayerSave(int playerId) { return null; }
    }
}
