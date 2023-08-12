using UniverseRift.Controllers.Players.Inventories.Items;

namespace UniverseRift.GameModels.Items
{
    public class ItemModel : BaseModel
    {
        public ItemType Type;
        public string SetName;
        public List<Bonus> Bonuses = new List<Bonus>();
        public string Rating;
    }
}
