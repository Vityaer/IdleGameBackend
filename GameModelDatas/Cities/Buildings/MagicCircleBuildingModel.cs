using Models.Data.Inventories;

namespace UniverseRift.GameModelDatas.Cities.Buildings
{
    public class MagicCircleBuildingModel : BuildingModel
    {
        public ResourceData HireCost;
        public Dictionary<string, float> SubjectChances = new();
        public Dictionary<string, float> Items = new();
        public Dictionary<string, float> Splinters = new();
    }
}
