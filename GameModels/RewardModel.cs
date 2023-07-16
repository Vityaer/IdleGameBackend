namespace UniverseRift.GameModels
{
    public class RewardModel : BaseModel
    {
        public List<ResourceData> Resources = new List<ResourceData>();
        public List<ItemData> Items = new List<ItemData>();
        public List<SplinterData> Splinters = new List<SplinterData>();
    }
}