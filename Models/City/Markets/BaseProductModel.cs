using Models.Data.Inventories;
using UniverseRift.GameModels;

namespace Models.City.Markets
{
    [Serializable]
    public class BaseProductModel : BaseModel
    {
        public int CountSell;
        public ResourceData Cost;
        public InventoryBaseItem Subject;
    }
}
