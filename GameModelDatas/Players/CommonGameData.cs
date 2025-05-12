using Models.Data.Inventories;
using UniverseRift.GameModelDatas.Cities;
using UniverseRift.Models.Misc;
using UniverseRift.Models.Misc.Temp;
using UniverseRift.Models.Rewards;

namespace UniverseRift.GameModelDatas.Players
{
    public class CommonGameData
    {
        public CityData City = new CityData();
        public PlayerData PlayerInfoData = new PlayerData();
        public AchievmentStorageData AchievmentStorage = new();
        public CycleEventsData CycleEventsData = new CycleEventsData();
        public HeroesStorage HeroesStorage = new HeroesStorage();
        public List<ResourceData> Resources = new();
        public InventoryData InventoryData = new();
        public BattlepasData BattlepasData;
        public CommunicationData CommunicationData = new();
        public TemporallyData TemporallyData = new();
        public Dictionary<string, string> Teams = new();
    }
}
