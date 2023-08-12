using Models.City.Markets;
using UniverseRift.GameModels;

namespace UniverseRift.Services.Rewarders
{
    public interface IRewardService
    {
        public async Task AddReward(int playerId, RewardModel reward) { }
        public async Task AddProduct(int playerId, BaseProductModel produt) { }

    }
}
