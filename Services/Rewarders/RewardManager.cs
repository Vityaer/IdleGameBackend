using Models.City.Markets;
using Models.Data.Inventories;
using UniverseRift.Controllers.Players.Inventories.Items;
using UniverseRift.GameModels;
using UniverseRift.Models.Resources;
using UniverseRift.Services.Resources;

namespace UniverseRift.Services.Rewarders
{
    public class RewardManager : IRewardService
    {
        private readonly IResourceManager _resourceController;
        private readonly IItemsController _itemsController;

        public RewardManager(
            IResourceManager resourceController,
            IItemsController itemsController
            )
        {
            _itemsController = itemsController;
            _resourceController = resourceController;
        }

        public async Task AddReward(int playerId, RewardModel reward)
        {
            foreach (var res in reward.Resources)
            {
                var resourse = new Resource { PlayerId = playerId, Type = res.Type, Count = res.Amount.Mantissa, E10 = res.Amount.E10 };
                await _resourceController.AddResources(resourse);
            }

            foreach (var item in reward.Items)
            {
                await _itemsController.AddItem(playerId, item.Id, item.Amount);
            }
        }

        public async Task AddProduct(int playerId, BaseProductModel product)
        {
            switch (product)
            {
                case ResourceProductModel ResourceProduct:
                    var gameResource = new GameResource(ResourceProduct.Subject);
                    var playerResorce = new Resource(playerId, gameResource);
                    await _resourceController.AddResources(playerResorce);
                    break;
                case ItemProductModel ItemProduct:
                    await _itemsController.AddItem(playerId, ItemProduct.Id, ItemProduct.Subject.Amount);
                    break;
            }
        }
    }
}
