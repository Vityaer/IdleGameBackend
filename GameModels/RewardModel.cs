using Models.Data.Inventories;

namespace UniverseRift.GameModels
{
    public class RewardModel : BaseModel
    {
        public List<ResourceData> Resources = new List<ResourceData>();
        public List<ItemData> Items = new List<ItemData>();
        public List<SplinterData> Splinters = new List<SplinterData>();

        public void Add<T>(T subject) where T : InventoryBaseItem
        {
            switch(subject)
            {
                case ResourceData resource:
                    Resources.Add(resource);
                    break;
                case ItemData item:
                    Items.Add(item);
                    break;
                case SplinterData splinter:
                    Splinters.Add(splinter);
                    break;
            }
        }
    }
}