using UniverseRift.GameModelDatas.Cities;
using UniverseRift.Models.City.Markets;

namespace UniverseRift.Controllers.Buildings.Achievments
{
    public interface IAchievmentController
    {
        Task<AchievmentStorageData> GetPlayerSave(int playerId);
        Task ClearDailyTask();
    }
}
