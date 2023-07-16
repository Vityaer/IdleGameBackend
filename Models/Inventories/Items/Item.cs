using UniverseRift.Controllers.Players.Inventories.Items;
using UniverseRift.Models.Common;

namespace UniverseRift.Models.Items
{
    public class Item : BaseInventoryObject
    {
        public string Name { get; set; } = string.Empty;

        public Item() { }

        public Item(int playerId, string name, int count = 1)
        {
            PlayerId = playerId;
            Name = name;
            Count = count;
            E10 = 0;
        }
    }
}
