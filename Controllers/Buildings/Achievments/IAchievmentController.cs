using UniverseRift.GameModelDatas.Cities;

namespace UniverseRift.Controllers.Buildings.Achievments
{
    public interface IAchievmentController
    {
        Task<AchievmentStorageData> GetPlayerSave(int playerId);
        Task AchievmentUpdataData(int playerId, string modelId, int amount);
        Task RefreshDailyTask();
    }
}
