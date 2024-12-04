using Microsoft.AspNetCore.Mvc;
using UniverseRift.Controllers.Players.Inventories.Items;
using UniverseRift.Controllers.Players.Inventories.Splinters;
using UniverseRift.Models.Common;
using UniverseRift.Models.Resources;
using UniverseRift.Services.Resources;

namespace UniverseRift.Controllers.Cheats
{
    public class CheatsController : ControllerBase
    {
        private readonly IResourceManager _resourceController;
        private readonly IItemsController _itemsController;
        private readonly ISplinterController _splinterController;

        public CheatsController(
            IResourceManager resourceController,
            IItemsController itemsController,
            ISplinterController splinterController
            )
        {
            _itemsController = itemsController;
            _resourceController = resourceController;
            _splinterController = splinterController;
        }

        [HttpPost]
        [Route("Cheats/AddAllResources")]
        public async Task AddResources(int playerId)
        {
            var reward = new List<Resource>();
            foreach (var type in (ResourceType[])Enum.GetValues(typeof(ResourceType)))
            {
                var resource = new Resource { PlayerId = playerId, Type = type, Count = 1, E10 = 3 };
                reward.Add(resource);
            }

            await _resourceController.AddResources(reward);
        }

        [HttpPost]
        [Route("Cheats/AddResources")]
        public async Task AddResources(int playerId, string type, float mantissa, int e10)
        {
            if (Enum.TryParse<ResourceType>(type, out var resourceType))
            {
                var resource = new Resource { PlayerId = playerId, Type = resourceType, Count = mantissa, E10 = e10 };
                await _resourceController.AddResources(resource);
            }
        }

        [HttpPost]
        [Route("Cheats/AddItems")]
        public async Task AddItems(int playerId, string itemId, int amount)
        {
            await _itemsController.AddItem(playerId, itemId, amount);
        }

        [HttpPost]
        [Route("Cheats/AddSplinters")]
        public async Task AddSplinters(int playerId, string splinterId, int amount)
        {
            await _splinterController.AddSplinters(playerId, splinterId, amount);
        }
    }
}
