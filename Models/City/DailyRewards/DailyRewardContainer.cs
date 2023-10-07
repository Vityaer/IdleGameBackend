using Models.Data.Inventories;
using UniverseRift.GameModelDatas.Cities;

namespace UniverseRift.Models.City.DailyRewards
{
    public class DailyRewardContainer : SimpleBuildingData
    {
        public List<InventoryBaseItem> Rewards = new List<InventoryBaseItem>();
    }
}
