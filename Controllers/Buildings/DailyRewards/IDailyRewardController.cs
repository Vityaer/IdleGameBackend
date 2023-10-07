using UniverseRift.GameModelDatas.Cities;
using UniverseRift.Models.City.DailyRewards;

namespace UniverseRift.Controllers.Buildings.DailyRewards
{
    public interface IDailyRewardController
    {
        Task<DailyRewardContainer> GetPlayerSave(int playerId, bool flagCreateNewData);
    }
}
