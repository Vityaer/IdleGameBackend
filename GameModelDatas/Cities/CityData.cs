using UniverseRift.GameModelDatas.Cities.Buildings;
using UniverseRift.GameModelDatas.Cities.Industries;
using UniverseRift.GameModelDatas.Cities.TravelCircleRaces;
using UniverseRift.Models.City.DailyRewards;
using UniverseRift.Models.City.FortuneWheels;
using UniverseRift.Models.City.Markets;
using UniverseRift.Models.City.TaskBoards;
using UniverseRift.Models.Guilds;
using UniverseRift.Models.LongTravels;

namespace UniverseRift.GameModelDatas.Cities
{
    public class CityData
    {
        public TimeManagementData TimeManagementSave = new TimeManagementData();
        public IndustryData IndustrySave = new IndustryData();
        public MarketData MallSave = new MarketData();
        public BuildingWithFightTeamsData ChallengeTowerSave = new BuildingWithFightTeamsData();
        public MainCampaignBuildingData MainCampaignSave = new();
        public TravelBuildingData TravelCircleSave = new TravelBuildingData();
        public GuildPlayerSaveContainer GuildPlayerSaveContainer = new GuildPlayerSaveContainer();
        public VoyageBuildingData VoyageSave = new VoyageBuildingData();
        public ArenaBuildingModel ArenaSave;
        public SimpleBuildingData Tutorial = new SimpleBuildingData();
        public DailyRewardContainer DailyReward = new DailyRewardContainer();
        public FortuneWheelData FortuneWheelData = new FortuneWheelData();
        public TaskBoardData TaskBoardData = new TaskBoardData();
        public LongTravelData LongTravelData = new();
    }
}