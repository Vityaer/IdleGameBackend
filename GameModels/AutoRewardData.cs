using Models.Data.Inventories;
using UniverseRift.Models.Resources;

namespace UniverseRift.GameModels
{
    public class AutoRewardData : PosibleRewardData
    {
        public Dictionary<ResourceType, ResourceData> BaseResource;

        public RewardModel GetCaculateReward(int countTact, int playerId)
        {
            var reward = new RewardModel();
            foreach (var resourceData in BaseResource.Values)
            {
                var resource = new GameResource(resourceData.Type, resourceData.Amount) * countTact;
                reward.Resources.Add(new ResourceData { Type = resource.Type, Amount = resource.Amount });
            }
            return reward;
        }
    }
}