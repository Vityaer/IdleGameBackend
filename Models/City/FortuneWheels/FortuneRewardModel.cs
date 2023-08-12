using Models.Data.Inventories;
using UniverseRift.GameModels;

namespace Models.City.FortuneRewards
{
    public class FortuneRewardModel : BaseModel
    {
        public float Probability;
        public float FactorDelta;
        public InventoryBaseItem Subject;
    }
}