using UniverseRift.GameModelDatas.Cities;
using UniverseRift.Models.DailyTasks;

namespace UniverseRift.Controllers.Buildings.Voyages
{
    public interface IVoyageController
    {
        Task<VoyageBuildingData> GetPlayerSave(int playerId);
        Task NextDay();
    }
}
