using Models.Data.Inventories;
using UniverseRift.GameModels;

namespace UniverseRift.GameModelDatas.Cities.Industries
{
    public class MineModel : BaseModel
    {
        public MineType Type;
        public string SpritePath;
        public CostLevelUpContainer IncomesContainer;
        public CostLevelUpContainer CostLevelUpContainer;
        public List<ResourceData> CreateCost = new List<ResourceData>();
    }
}