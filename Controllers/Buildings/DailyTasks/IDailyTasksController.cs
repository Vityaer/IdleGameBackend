using UniverseRift.GameModelDatas.Cities;
using UniverseRift.Models.DailyTasks;

namespace UniverseRift.Controllers.Buildings.DailyTasks
{
    public interface IDailyTasksController
    {
        async Task<DailyTaskContainer> GetPlayerSave(int playerId) { return null; }

    }
}
