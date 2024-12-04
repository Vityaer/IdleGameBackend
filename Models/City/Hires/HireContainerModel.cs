using Models.Data.Inventories;
using UniverseRift.GameModels;

namespace Models.City.Hires
{
    public class HireContainerModel : BaseModel
    {
        public ResourceData Cost;
        public List<HireModel> ChanceHires = new();
    }
}
