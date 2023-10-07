using Microsoft.AspNetCore.Mvc;
using UniverseRift.Controllers.Players.Inventories.Items;
using UniverseRift.Models.Common;
using UniverseRift.Models.Resources;
using UniverseRift.Services.Resources;

namespace UniverseRift.Controllers.Cheats
{
    public class CheatsController : ControllerBase
    {
        private readonly IResourceManager _resourceController;
        private readonly IItemsController _itemsController;

        public CheatsController(
            IResourceManager resourceController,
            IItemsController itemsController
            )
        {
            _itemsController = itemsController;
            _resourceController = resourceController;
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

    }
}
