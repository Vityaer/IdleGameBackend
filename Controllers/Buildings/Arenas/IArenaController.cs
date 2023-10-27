using UniverseRift.GameModelDatas.Cities;

namespace UniverseRift.Controllers.Buildings.Arenas
{
    public interface IArenaController
    {
        Task<ArenaBuildingModel> GetPlayerSave(int playerId);
    }
}
