using UniverseRift.GameModels;

namespace UniverseRift.Controllers.Services.Rewarders
{
    public interface IClientRewardService
    {
        public async Task AddReward(int playerId, RewardModel reward) { }
    }
}
