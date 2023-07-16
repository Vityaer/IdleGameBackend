using UniverseRift.Controllers.Players.Inventories.Resources;
using UniverseRift.GameModels;
using UniverseRift.Models.Resources;

namespace UniverseRift.Controllers.Services.Rewarders
{
    public class ClientRewardService : IClientRewardService
    {
        private readonly IResourceController _resourceController;

        public ClientRewardService(IResourceController resourceController) 
        {
            _resourceController = resourceController;
        }

        public async Task AddReward(int playerId, RewardModel reward)
        {
            foreach (var res in reward.Resources)
            {
                var resourse = new Resource { PlayerId = playerId, Type = res.Type, Count = res.Amount.Mantissa, E10 = res.Amount.E10};
                await _resourceController.AddResources(resourse);
            }
        }
    }
}
