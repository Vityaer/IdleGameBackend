using UniverseRift.GameModelDatas.Cities;

namespace UniverseRift.Controllers.Buildings.DailyRewards
{
    public interface IDailyRewardController
    {
        async Task<SimpleBuildingData> GetPlayerSave(int playerId) { return null; }
    }
}
