using Models.City.Markets;
using UniverseRift.GameModels;

namespace UniverseRift.Services.Rewarders
{
    public interface IRewardService
    {
        public Task AddReward(int playerId, RewardModel reward);
        public Task AddProduct(int playerId, BaseProductModel produt);

    }
}
