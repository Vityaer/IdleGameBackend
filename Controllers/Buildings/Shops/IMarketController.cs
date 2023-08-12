using UniverseRift.Models.City.Markets;

namespace UniverseRift.Controllers.Buildings.Shops
{
    public interface IMarketController
    {
        Task<MarketData> GetPlayerSave(int playerId);
    }
}
