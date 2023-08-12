using Models.Data.Inventories;
using UniverseRift.GameModelDatas.Cities;

namespace UniverseRift.GameModelDatas.Players
{
    public class CommonGameData
    {
        public CityData City = new CityData();
        public PlayerData PlayerInfoData = new PlayerData();
        public AchievmentStorageData AchievmentStorage = new();
        public CycleEventsData CycleEventsData = new CycleEventsData();
        public HeroesStorage HeroesStorage = new HeroesStorage();
        public List<ResourceData> Resources;
        public InventoryData InventoryData = new();
    }
}
