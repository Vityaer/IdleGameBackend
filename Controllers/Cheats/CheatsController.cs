using Microsoft.AspNetCore.Mvc;
using UniverseRift.Models.Resources;
using UniverseRift.Services.Resources;

namespace UniverseRift.Controllers.Cheats
{
    public class CheatsController : ControllerBase
    {
        private readonly IResourceManager _resourceController;

        public CheatsController(IResourceManager resourceController)
        {
            _resourceController = resourceController;
        }

        [HttpPost]
        [Route("Cheats/AddResources")]
        public async Task RemoveHeroes(int playerId, string type, float mantissa, int e10)
        {
            if (Enum.TryParse<ResourceType>(type, out var resourceType))
            {
                var resource = new Resource { PlayerId = playerId, Type = resourceType, Count = mantissa, E10 = e10 };
                await _resourceController.AddResources(resource);
            }
        }
    }
}
