using UniverseRift.GameModelDatas.Cities.Buildings;

namespace Models.City.Mines
{
    public class MineBuildingModel : BuildingModel
    {
        public List<MineEnergyDataModel> MineEnergyDatas = new();
        public List<MineSettingsCampaignContainer> SettingsCampaigns = new();
    }
}
