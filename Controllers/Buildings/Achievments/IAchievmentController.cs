using UniverseRift.GameModelDatas.Cities;

namespace UniverseRift.Controllers.Buildings.Achievments
{
    public interface IAchievmentController
    {
        async Task<AchievmentStorageData> GetPlayerSave(int playerId) { return null; }
    }
}
