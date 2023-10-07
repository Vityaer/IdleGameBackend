using UniverseRift.GameModelDatas.Cities.Industries;
using UniverseRift.GameModelDatas.Players;
using UniverseRift.Models.City.DailyRewards;
using UniverseRift.Models.City.FortuneWheels;
using UniverseRift.Models.City.Markets;
using UniverseRift.Models.City.TaskBoards;
using UniverseRift.Models.DailyTasks;

namespace UniverseRift.GameModelDatas.Cities
{
    public class CityData
    {
        public TimeManagementData TimeManagementSave = new TimeManagementData();
        public IndustryData IndustrySave = new IndustryData();
        public MarketData MallSave = new MarketData();
        public BuildingWithFightTeamsData ChallengeTowerSave = new BuildingWithFightTeamsData();
        public BuildingWithFightTeamsData MainCampaignSave = new BuildingWithFightTeamsData();
        public BuildingWithFightTeamsData TravelCircleSave = new BuildingWithFightTeamsData();
        public BuildingWithFightTeamsData GildSave = new BuildingWithFightTeamsData();
        public VoyageBuildingData VoyageSave = new VoyageBuildingData();
        public ArenaBuildingModel ArenaSave = new ArenaBuildingModel();
        public SimpleBuildingData Tutorial = new SimpleBuildingData();
        public DailyRewardContainer DailyReward = new DailyRewardContainer();
        public DailyTaskContainer DailyTaskContainer = new DailyTaskContainer();
        public FortuneWheelData FortuneWheelData = new FortuneWheelData();
        public TaskBoardData TaskBoardData = new TaskBoardData();
    }
}