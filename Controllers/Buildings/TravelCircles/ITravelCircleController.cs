using UniverseRift.GameModelDatas.Cities;
using UniverseRift.GameModelDatas.Cities.TravelCircleRaces;

namespace UniverseRift.Controllers.Buildings.TravelCircles
{
    public interface ITravelCircleController
    {
        Task<TravelBuildingData> GetPlayerSave(int playerId, bool flagCreateNewData);
        Task OnRegistrationPlayer(int playerId);
    }
}
