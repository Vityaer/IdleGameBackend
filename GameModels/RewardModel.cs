using Models.Data.Inventories;

namespace UniverseRift.GameModels
{
    public class RewardModel : BaseModel
    {
        public List<ResourceData> Resources = new();
        public List<ItemData> Items = new();
        public List<SplinterData> Splinters = new();

        public void Add<T>(T subject) where T : InventoryBaseItem
        {
            switch(subject)
            {
                case ResourceData resource:
                    var oldResource = Resources.Find(res => res.Type.Equals(resource.Type));
                    if (oldResource != null)
                    {
                        oldResource.Amount.Add(resource.Amount);
                    }
                    else
                    {
                        Resources.Add(resource);
                    }
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