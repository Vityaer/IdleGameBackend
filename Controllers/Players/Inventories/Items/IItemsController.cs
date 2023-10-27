namespace UniverseRift.Controllers.Players.Inventories.Items
{
    public interface IItemsController
    {
        Task AddItem(int playerId, string itemName, int count = 1);
        Task RemoveItem(int playerId, string itemName, int count = 1);
    }
}
