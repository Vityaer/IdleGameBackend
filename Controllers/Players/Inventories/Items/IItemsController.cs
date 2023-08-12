namespace UniverseRift.Controllers.Players.Inventories.Items
{
    public interface IItemsController
    {
        async Task AddItem(int playerId, string itemName, int count = 1){}
        async Task RemoveItem(int playerId, string itemName, int count = 1) { }
    }
}
