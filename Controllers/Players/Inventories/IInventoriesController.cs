using UniverseRift.GameModelDatas.Players;

namespace UniverseRift.Controllers.Players.Inventories
{
    public interface IInventoriesController
    {
        Task<InventoryData> GetInventory(int playerId);
    }
}
