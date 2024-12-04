using UniverseRift.GameModelDatas.Cities;

namespace UniverseRift.Controllers.Buildings.Campaigns
{
    public interface ICampaignController
    {
        Task CreatePlayerProgress(int playerId);

        Task<MainCampaignBuildingData> GetPlayerSave(int playerId);
    }
}
