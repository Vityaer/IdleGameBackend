using Models.Data.Inventories;
using UniverseRift.GameModelDatas.Cities.Buildings;
using UniverseRift.GameModels;

namespace Models.City.MagicCircles
{
    public class MagicCircleBuildingModel : BuildingModel
    {
        public ResourceData HireCost;
        public Dictionary<string, float> SubjectChances = new();
        public PosibleRewardData PosibleRewardData = new();
    }
}
