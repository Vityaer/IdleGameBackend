using UniverseRift.GameModelDatas.Cities;

namespace UniverseRift.Controllers.Buildings.TravelCircles
{
    public interface ITravelCircleController
    {
        Task<BuildingWithFightTeamsData> GetPlayerSave(int playerId, bool flagCreateNewData);
    }
}
