using UniverseRift.Contexts;
using UniverseRift.Models.Players;

namespace UniverseRift.Controllers.Buildings.Campaigns
{
    public interface ICampaignController
    {
        AplicationContext Context { get; }
        async Task CreatePlayerProgress(int playerId)
        {
        }
    }
}
