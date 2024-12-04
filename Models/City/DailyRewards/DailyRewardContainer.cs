using Models.Data.Inventories;
using UniverseRift.GameModelDatas.Cities;

namespace UniverseRift.Models.City.DailyRewards
{
    public class DailyRewardContainer : SimpleBuildingData
    {
        public int CurrentDailyReward = 0;
        public bool CanGetDailyReward = false;
        public List<InventoryBaseItem> Rewards = new List<InventoryBaseItem>();
    }
}
