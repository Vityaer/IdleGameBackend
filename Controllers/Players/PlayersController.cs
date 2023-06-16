using Microsoft.AspNetCore.Mvc;
using UniverseRift.Contexts;
using UniverseRift.Controllers.Players.Inventories.Resources;
using UniverseRift.Models.Common;
using UniverseRift.Models.Results;

namespace UniverseRift.Controllers.Players
{
    public class PlayersController : ControllerBase, IPlayersController
    {
        private readonly AplicationContext _context;
        private readonly IResourceController _resourcesController;

        public PlayersController(AplicationContext context, IResourceController resourcesController)
        {
            _context = context;
            _resourcesController = resourcesController;
        }

        public async Task<Player> GetPlayer(int playerId)
        {
            var result = await _context.Players.FindAsync(playerId);
            return result;
        }

        [HttpPost]
        [Route("Players/Registration")]
        public async Task<int> Registration(string name)
        {
            var player = new Player { Name = name };
            _context.Add(player);
            await _context.SaveChangesAsync();
            await _resourcesController.CreateResources(player.Id);
            return player.Id;
        }

        [HttpPost]
        [Route("Players/PlayerRename")]
        public async Task<AnswerModel> PlayerRename(int playerId, string newName)
        {
            var answer = new AnswerModel();

            var player = await GetPlayer(playerId);
            player.Name = newName;
            await _context.SaveChangesAsync();
        }
    }
}
