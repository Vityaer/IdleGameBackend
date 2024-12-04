using Models.Data.Inventories;
using UIController.Rewards.PosibleRewards;
using UniverseRift.Controllers.Common;
using UniverseRift.Models.Resources;

namespace UniverseRift.GameModels
{
    public class AutoRewardData : PosibleRewardData
    {
        public Dictionary<ResourceType, ResourceData> BaseResource;
        private static Random _random;

        public RewardModel GetCaculateReward(int countTact, int playerId)
        {
            if (_random == null)
                _random = new Random();

            var reward = new RewardModel();
            foreach (var resourceData in BaseResource.Values)
            {
                var resource = new GameResource(resourceData.Type, resourceData.Amount) * countTact;
                reward.Resources.Add(new ResourceData { Type = resource.Type, Amount = resource.Amount });
            }

            var amount = 0;
            foreach (var subject in Objects)
            {
                switch (subject)
                {
                    case PosibleObjectData<ResourceData> resourceData:
                        amount = GetRandomAmount(resourceData.Posibility, countTact, _random);
                        if (amount > 0)
                        {
                            var resource = new GameResource(resourceData.Value.Type, resourceData.Value.Amount) * countTact;
                            reward.Resources.Add(new ResourceData { Type = resource.Type, Amount = resource.Amount });
                        }
                        break;
                    case PosibleObjectData<ItemData> itemData:
                        amount = GetRandomAmount(itemData.Posibility, countTact, _random);
                        if (amount > 0)
                        {
                            reward.Items.Add(new ItemData { Id = itemData.Value.Id, Amount = amount });
                        }
                        break;

                    case PosibleObjectData<SplinterData> splinterData:
                        amount = GetRandomAmount(splinterData.Posibility, countTact, _random);
                        if (amount > 0)
                        {
                            reward.Splinters.Add(new SplinterData { Id = splinterData.Value.Id, Amount = amount });
                        }
                        break;
                }

            }

            return reward;
        }

        private int GetRandomAmount(int posibility, int countTact, Random random)
        {
            var amount = 0;

            for (var i = 0; i < countTact; i++)
                if (random.Next(Constants.Common.MAX_RANDOM) <= posibility)
                    amount += 1;

            return amount;
        }
    }

}