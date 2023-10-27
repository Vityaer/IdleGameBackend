using UniverseRift.GameModelDatas.Cities;
using UniverseRift.Models.Rewards;

namespace UniverseRift.Controllers.Buildings.Battlepases
{
    public interface IBattlepasController
    {
        Task<BattlepasData> GetPlayerSave(int playerId, bool flagNewDay);
        Task OnRegisterPlayer(int playerId);
    }
}
