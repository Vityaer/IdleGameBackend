using UniverseRift.GameModelDatas.Cities;

namespace UniverseRift.Controllers.Buildings.TimeMenagers
{
    public interface ITimeManagerController
    {
        async Task<TimeManagementData> GetPlayerSave(int playerId) { return null; }
    }
}
