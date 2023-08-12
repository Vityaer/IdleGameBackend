using UniverseRift.GameModelDatas.Cities;
using UniverseRift.Models.DailyTasks;

namespace UniverseRift.Controllers.Buildings.Voyages
{
    public interface IVoyageController
    {
        async Task<VoyageBuildingData> GetPlayerSave(int playerId) { return null; }
    }
}
