using Microsoft.AspNetCore.Mvc;
using Misc.Json;
using Models.Data.Inventories;
using UniRx;
using UniverseRift.Contexts;
using UniverseRift.Controllers.Buildings.Battlepases;
using UniverseRift.Controllers.Buildings.Campaigns;
using UniverseRift.Controllers.Buildings.Guilds;
using UniverseRift.Controllers.Buildings.Industries;
using UniverseRift.Controllers.Buildings.Industries.Mines;
using UniverseRift.Controllers.Buildings.LongTravels;
using UniverseRift.Controllers.Buildings.TravelCircles;
using UniverseRift.Controllers.Common;
using UniverseRift.Controllers.Misc.Friendships;
using UniverseRift.GameModelDatas.Players;
using UniverseRift.GameModels;
using UniverseRift.GameModels.Common;
using UniverseRift.Models.Common;
using UniverseRift.Models.Resources;
using UniverseRift.Models.Results;
using UniverseRift.Services.Resources;
using UniverseRift.Services.Rewarders;

namespace UniverseRift.Controllers.Players
{
    public class PlayersController : Controller, IPlayersController
    {
        private readonly AplicationContext _context;
        private readonly IResourceManager _resourcesController;
        private readonly ICommonDictionaries _commonDictionaries;
        private readonly IRewardService _clientRewardService;
        private readonly ICampaignController _campaignController;
        private readonly IJsonConverter _jsonConverter;
        private readonly IMineController _mineController;
        private readonly ITravelCircleController _travelCircleController;
        private readonly IBattlepasController _battlepasController;
        private readonly IFriendshipController _friendshipController;
        private readonly IGuildController _guildController;
        private readonly ILongTravelController _longTravelController;
        private readonly IIndustryController _industryController;

		private ReactiveCommand<int> _onPlayerRegistration = new();

        public UniRx.IObservable<int> OnRegistrationPlayer => _onPlayerRegistration;

        public PlayersController(
            AplicationContext context,
            IResourceManager resourcesController,
            ICommonDictionaries commonDictionaries,
            IRewardService clientRewardService,
            ICampaignController campaignController,
            IJsonConverter jsonConverter,
            IMineController mineController,
            ITravelCircleController travelCircleController,
            IBattlepasController battlepasController,
            IFriendshipController friendshipController,
            IGuildController guildController,
            ILongTravelController longTravelController,
			IIndustryController industryController
			)
        {
            _commonDictionaries = commonDictionaries;
            _context = context;
            _jsonConverter = jsonConverter;
            _resourcesController = resourcesController;
            _clientRewardService = clientRewardService;
            _campaignController = campaignController;
            _mineController = mineController;
            _travelCircleController = travelCircleController;
            _battlepasController = battlepasController;
            _friendshipController = friendshipController;
            _guildController = guildController;
            _longTravelController = longTravelController;
			_industryController = industryController;
		}

        public async Task<Player> CreatePlayer(string name, string avatarPath, bool isBot)
        {
            var player = new Player
            {
                Name = name,
                AvatarPath = avatarPath,
                LastGetAlchemyDateTime = DateTime.UtcNow.ToString(Constants.Common.DateTimeFormat),
                IsBot = isBot
            };
            await _context.AddAsync(player);

            await _context.SaveChangesAsync();
            await _resourcesController.CreateResources(player.Id);
            var rewardData = _commonDictionaries.Rewards["Registration"];
            await _clientRewardService.AddReward(player.Id, rewardData);

            await _campaignController.CreatePlayerProgress(player.Id);
            await _mineController.OnRegistrationPlayer(player.Id);
            await _travelCircleController.OnRegistrationPlayer(player.Id);
            await _battlepasController.OnRegisterPlayer(player.Id);
            await _friendshipController.OnPlayerRegister(player.Id);
            await _guildController.OnPlayerRegister(player.Id);
            await _longTravelController.OnPlayerRegister(player.Id);
            await _industryController.OnPlayerRegister(player.Id);
			return player;
        }

        public async Task<Player> GetPlayer(int playerId)
        {
            var player = await _context.Players.FindAsync(playerId);
            return player;
        }

        [HttpPost]
        [Route("Players/Registration")]
        public async Task<AnswerModel> Registration(string name, string avatarPath)
        {
            var answer = new AnswerModel();
            if (string.IsNullOrEmpty(avatarPath))
                avatarPath = string.Empty;

            var player = await CreatePlayer(name, avatarPath, false);

            answer.Result = player.Id.ToString();
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

        [HttpPost]
        [Route("Players/ChangeAvatar")]
        public async Task<AnswerModel> ChangeAvatar(int playerId, string avatarPath)
        {
            var answer = new AnswerModel();

            var player = await GetPlayer(playerId);
            player.AvatarPath = avatarPath;
            await _context.SaveChangesAsync();
            answer.Result = "Success";
            return answer;
        }

        [HttpPost]
        [Route("Players/PlayerLevelUp")]
        public async Task<AnswerModel> PlayerLevelUp(int playerId)
        {
            var answer = new AnswerModel();

            var player = await GetPlayer(playerId);

            if (player == null)
            {
                answer.Error = "Wrong data";
                return answer;
            }
            var requireExp = _commonDictionaries.CostContainers["PlayerLevels"].GetCostForLevelUp(player.Level, playerId)[0];
            var enoughResource = await _resourcesController.CheckResource(playerId, requireExp, answer);

            if (!enoughResource)
                return answer;

            player.Level += 1;

            var resoureData = new ResourceData { Type = ResourceType.Diamond, Amount = new BigDigit(100) };
            var gameResource = new GameResource(resoureData);
            var rewardResource = new Resource(playerId, gameResource);
            await _resourcesController.AddResources(rewardResource);

            await _context.SaveChangesAsync();

            var rewardModel = new RewardModel();
            rewardModel.Add(resoureData);

            answer.Result = _jsonConverter.Serialize(rewardModel);
            return answer;
        }

        public async Task<PlayerData> GetPlayerSave(int playerId)
        {
            var player = await GetPlayer(playerId);
            var result = new PlayerData(player);
            return result;
        }
    }
}
