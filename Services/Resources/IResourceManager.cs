using Models.Data.Inventories;
using UniverseRift.Models.Resources;
using UniverseRift.Models.Results;

namespace UniverseRift.Services.Resources
{
    public interface IResourceManager
    {
        Task<bool> CheckResource(int playerId, Resource resource, AnswerModel answer);
        Task<bool> CheckResource(int playerId, List<Resource> resources, AnswerModel answer);
        Task CreateResources(int playerId);
        Task SubstactResources(Resource res);
        Task SubstactResources(List<Resource> resources);
        Task AddResources(Resource res);
        Task AddResources(List<Resource> resources);

        Task<List<ResourceData>> GetPlayerSave(int playerId);
    }
}
