using Models.Data.Inventories;

namespace UniverseRift.GameModels
{
    public class CostLevelUpModel
    {
        public int level;
        public List<ResourceData> Cost;
        public CostIncreaseType typeIncrease = CostIncreaseType.Mulitiply;
        public List<float> ListIncrease = new List<float>();
    }
}