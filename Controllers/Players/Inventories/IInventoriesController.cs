using UniverseRift.GameModelDatas.Players;
using UniverseRift.Models.Common;

namespace UniverseRift.Controllers.Players.Inventories
{
    public interface IInventoriesController
    {
        public async Task<InventoryData> GetInventory(int playerId) { return null; }
    }
}
