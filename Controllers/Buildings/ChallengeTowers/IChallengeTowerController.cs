using UniverseRift.GameModelDatas.Cities;

namespace UniverseRift.Controllers.Buildings.ChallengeTowers
{
    public interface IChallengeTowerController
    {
        Task<BuildingWithFightTeamsData> GetPlayerSave(int playerId);
    }
}
