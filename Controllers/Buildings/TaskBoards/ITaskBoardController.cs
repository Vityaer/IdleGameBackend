using UniverseRift.Models.City.TaskBoards;

namespace UniverseRift.Controllers.Buildings.TaskBoards
{
    public interface ITaskBoardController
    {
        Task<TaskBoardData> GetPlayerSave(int playerId);
    }
}
