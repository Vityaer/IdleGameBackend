using UniverseRift.Models.Resources;
using UniverseRift.Models.Results;

namespace UniverseRift.Controllers.Players.Inventories.Resources
{
    public interface IResourceController
    {
        public async Task<bool> CheckResource(int playerId, Resource resource, AnswerModel answer) { return false; }
        public async Task<bool> CheckResource(int playerId, List<Resource> resources, AnswerModel answer) { return false; }
        public async Task CreateResources(int playerId) { }
        public async Task SubstactResources(Resource res) { }
        public async Task AddResources(Resource res) { }
    }
}
