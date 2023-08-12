using UniverseRift.Contexts;
using UniverseRift.GameModelDatas.Cities;
using UniverseRift.Models.Players;

namespace UniverseRift.Controllers.Buildings.Campaigns
{
    public interface ICampaignController
    {
        async Task CreatePlayerProgress(int playerId) { }

        async Task<BuildingWithFightTeamsData> GetPlayerSave(int playerId) { return null; }
    }
}
