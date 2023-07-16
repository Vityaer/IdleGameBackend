using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniRx;
using UniverseRift.Contexts;
using UniverseRift.Controllers.Buildings.Campaigns;
using UniverseRift.Controllers.Common;
using UniverseRift.Controllers.Players.Inventories.Resources;
using UniverseRift.Controllers.Services.Rewarders;
using UniverseRift.Models.Common;
using UniverseRift.Models.Results;

namespace UniverseRift.Controllers.Players
{
    public class PlayersController : Controller, IPlayersController
    {
        private readonly AplicationContext _context;
        private readonly IResourceController _resourcesController;
        private readonly ICommonDictionaries _commonDictionaries;
        private readonly IClientRewardService _clientRewardService;
        private readonly ICampaignController _campaignController;

        private ReactiveCommand<int> _onPlayerRegistration = new ReactiveCommand<int>();

        public UniRx.IObservable<int> OnRegistrationPlayer => _onPlayerRegistration;

        public PlayersController(
            AplicationContext context,
            IResourceController resourcesController,
            ICommonDictionaries commonDictionaries,
            IClientRewardService clientRewardService,
            ICampaignController campaignController
            )
        {
            _commonDictionaries = commonDictionaries;
            _context = context;
            _resourcesController = resourcesController;
            _clientRewardService = clientRewardService;
            _campaignController = campaignController;
        }

        public async Task<Player> GetPlayer(int playerId)
        {
            var result = await _context.Players.ToListAsync();
            var player = result.Find(player => player.Id == playerId);
            return player;
        }

        [HttpPost]
        [Route("Players/Registration")]
        public async Task<AnswerModel> Registration(string name)
        {
            var answer = new AnswerModel();
            var player = new Player { Name = name };
            await _context.AddAsync(player);

            await _context.SaveChangesAsync();
            await _resourcesController.CreateResources(player.Id);
            var rewardData = _commonDictionaries.Rewards["Registration"];
            await _clientRewardService.AddReward(player.Id, rewardData);

            await _campaignController.CreatePlayerProgress(player.Id);

            answer.Result = player.Id.ToString();
            var result = await _context.Players.ToListAsync();

            return answer;
        }

        [HttpPost]
        [Route("Players/PlayerRename")]
        public async Task<AnswerModel> PlayerRename(int playerId, string newName)
        {
            var answer = new AnswerModel();

            var player = await GetPlayer(playerId);
            player.Name = newName;
            await _context.SaveChangesAsync();
            answer.Result = "Success";
            return answer;
        }
    }
}
