using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Common.BigDigits;
using Models.Data.Inventories;
using UniverseRift.Contexts;
using UniverseRift.Models.Resources;
using UniverseRift.Models.Results;

namespace UniverseRift.Services.Resources
{
    public class ResourcesManager : Controller, IResourceManager
    {
        private readonly AplicationContext _context;

        public ResourcesManager(AplicationContext context)
        {
            _context = context;
        }

        public async Task CreateResources(int playerId)
        {
            foreach (var type in (ResourceType[])Enum.GetValues(typeof(ResourceType)))
            {
                await _context.Resources.AddAsync(new Resource { PlayerId = playerId, Type = type, Count = 0, E10 = 0 });
            }

            await _context.SaveChangesAsync();

            var result = await _context.Players.ToListAsync();
            var resources = await _context.Resources.ToListAsync();

        }

        public async Task AddResources(Resource newRes)
        {
            var resources = await _context.Resources.ToListAsync();
            var resource = resources.Find(res => res.PlayerId == newRes.PlayerId && res.Type == newRes.Type);

            if (resource == null)
                return;

            resource.Add(newRes);
            await _context.SaveChangesAsync();

        }

        public async Task SubstactResources(Resource newRes)
        {
            var resources = await _context.Resources.ToListAsync();
            var resource = resources.Find(res => res.PlayerId == newRes.PlayerId && res.Type == newRes.Type);

            if (resource == null)
                return;

            resource.Subtract(newRes);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CheckResource(int playerId, List<Resource> resources, AnswerModel answer)
        {
            var allResources = await _context.Resources.ToListAsync();

            var playerResource = new List<Resource>();
            foreach (var resource in resources)
            {
                var res = allResources.Find(res => res.PlayerId == playerId && res.Type == resource.Type);
                if (res != null)
                    playerResource.Add(res);
            }

            if (playerResource.Count == 0)
            {
                answer.Error = "Player havn't this resource";
                return false;
            }

            var enough = true;

            foreach (var resource in resources)
            {
                var res = playerResource.Find(res => res.Type == resource.Type);
                if (res != null)
                {
                    if (!res.CheckCount(resource.Count, resource.E10))
                    {
                        enough = false;
                        break;
                    }
                }
                else
                {
                    enough = false;
                    break;
                }
            }

            if (!enough)
            {
                answer.Error = "Not enough resources";
                return false;
            }

            return true;
        }

        public async Task<bool> CheckResource(int playerId, Resource resource, AnswerModel answer)
        {
            var resources = await _context.Resources.ToListAsync();
            var playerResource = resources.Find(res => res.PlayerId == playerId && res.Type == resource.Type);

            if (playerResource == null)
            {
                answer.Error = "Player havn't this resource";
                return false;
            }

            var enough = playerResource.CheckCount(resource.Count, resource.E10);

            if (!enough)
            {
                answer.Error = "Not enough resources";
                return false;
            }

            return true;
        }

        public async Task<List<ResourceData>> GetPlayerSave(int playerId)
        {
            var resources = await _context.Resources.ToListAsync();
            var playerResources = resources.FindAll(res => res.PlayerId == playerId);

            var result = new List<ResourceData>();
            foreach (var resource in playerResources)
            {
                result.Add(new ResourceData { Type = resource.Type, Amount = new BigDigit(resource.Count, resource.E10) });
            }

            return result;
        }
    }
}
