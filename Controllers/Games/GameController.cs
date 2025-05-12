using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Misc.Json;
using Models.Data.Inventories;
using UniverseRift.Contexts;
using UniverseRift.Controllers.Buildings.Achievments;
using UniverseRift.Controllers.Buildings.Arenas;
using UniverseRift.Controllers.Buildings.Battlepases;
using UniverseRift.Controllers.Buildings.Campaigns;
using UniverseRift.Controllers.Buildings.ChallengeTowers;
using UniverseRift.Controllers.Buildings.DailyRewards;
using UniverseRift.Controllers.Buildings.FortuneWheels;
using UniverseRift.Controllers.Buildings.GameCycles;
using UniverseRift.Controllers.Buildings.Guilds;
using UniverseRift.Controllers.Buildings.Industries;
using UniverseRift.Controllers.Buildings.LongTravels;
using UniverseRift.Controllers.Buildings.Shops;
using UniverseRift.Controllers.Buildings.TaskBoards;
using UniverseRift.Controllers.Buildings.TimeMenagers;
using UniverseRift.Controllers.Buildings.TravelCircles;
using UniverseRift.Controllers.Buildings.Tutorials;
using UniverseRift.Controllers.Buildings.Voyages;
using UniverseRift.Controllers.Common;
using UniverseRift.Controllers.Misc.Friendships;
using UniverseRift.Controllers.Misc.Mails;
using UniverseRift.Controllers.Players;
using UniverseRift.Controllers.Players.Heroes;
using UniverseRift.Controllers.Players.Inventories;
using UniverseRift.Controllers.Server;
using UniverseRift.GameModelDatas.Players;
using UniverseRift.GameModels.Common;
using UniverseRift.MessageData;
using UniverseRift.Models.City.FortuneWheels;
using UniverseRift.Models.City.TaskBoards;
using UniverseRift.Models.Common;
using UniverseRift.Models.FortuneWheels;
using UniverseRift.Models.Results;
using UniverseRift.Models.Rewards;
using UniverseRift.Models.Tasks;
using UniverseRift.Models.Tasks.SimpleTask;
using UniverseRift.Services.Resources;

namespace UniverseRift.Controllers.Games
{
    public class GameController : ControllerBase
    {
        private readonly IResourceManager _resourcesController;
        private readonly IJsonConverter _jsonConverter;
        private readonly ICampaignController _campaignController;
        private readonly IHeroesController _heroesController;
        private readonly IInventoriesController _inventoriesController;
        private readonly IPlayersController _playersController;
        private readonly IChallengeTowerController _challengeTowerController;
        private readonly ITaskBoardController _taskBoardController;
        private readonly IMarketController _mallController;
        private readonly IIndustryController _industryController;
        private readonly IGameCycleController _gameCycleController;
        private readonly IAchievmentController _achievmentController;
        private readonly IVoyageController _voyageController;
        private readonly IArenaController _arenaController;
        private readonly ITravelCircleController _travelCircleController;
        private readonly ITimeManagerController _timeManagerController;
        private readonly ITutorialController _tutorialController;
        private readonly IGuildController _guildController;
        private readonly IFortuneWheelController _fortuneWheelController;
        private readonly IServerController _serverController;
        private readonly ICommonDictionaries _commonDictionaries;
        private readonly IDailyRewardController _dailyRewardController;
        private readonly IBattlepasController _battlepasController;
        private readonly IFriendshipController _friendshipController;
        private readonly ILongTravelController _longTravelController;
        private readonly IMailController _mailController;
        private readonly AplicationContext _context;

        private bool _isCreated = false;
        private readonly Random _random = new Random();
        private const int REWARD_COUNT = 8;

        public GameController(
            IJsonConverter jsonConverter,
            IResourceManager resourcesController,
            ICampaignController campaignController,
            IHeroesController heroesController,
            IInventoriesController inventoriesController,
            IPlayersController playersController,
            IChallengeTowerController challengeTowerController,
            ITaskBoardController taskBoardController,
            IMarketController mallController,
            IIndustryController industryController,
            IGameCycleController gameCycleController,
            IAchievmentController achievmentController,
            IVoyageController voyageController,
            IArenaController arenaController,
            ITravelCircleController travelCircleController,
            ITimeManagerController timeManagerController,
            ITutorialController tutorialController,
            IGuildController guildController,
            IFortuneWheelController fortuneWheelController,
            ICommonDictionaries commonDictionaries,
            IServerController serverController,
            IBattlepasController battlepasController,
            IDailyRewardController dailyRewardController,
            IFriendshipController friendshipController,
            ILongTravelController longTravelController,
            IMailController mailController,
            AplicationContext context
            )
        {
            _context = context;
            _guildController = guildController;
            _tutorialController = tutorialController;
            _timeManagerController = timeManagerController;
            _travelCircleController = travelCircleController;
            _arenaController = arenaController;
            _voyageController = voyageController;
            _achievmentController = achievmentController;
            _gameCycleController = gameCycleController;
            _industryController = industryController;
            _mallController = mallController;
            _challengeTowerController = challengeTowerController;
            _jsonConverter = jsonConverter;
            _heroesController = heroesController;
            _resourcesController = resourcesController;
            _campaignController = campaignController;
            _playersController = playersController;
            _inventoriesController = inventoriesController;
            _taskBoardController = taskBoardController;
            _fortuneWheelController = fortuneWheelController;
            _serverController = serverController;
            _commonDictionaries = commonDictionaries;
            _dailyRewardController = dailyRewardController;
            _battlepasController = battlepasController;
            _friendshipController = friendshipController;
            _longTravelController = longTravelController;
            _mailController = mailController;
        }

        [HttpPost]
        [Route("GameController/GetPlayerSave")]
        public async Task<AnswerModel> GetPlayerSave(int playerId)
        {
            if (!_isCreated)
            {
                _isCreated = true;
                await _serverController.OnStartProject();
            }

            var answer = new AnswerModel();

            var playerSave = new CommonGameData();

            var result = await _context.Players.ToListAsync();
            var player = result.Find(player => player.Id == playerId);

            if (player == null)
            {
                answer.Error = "Player not found";
                return answer;
            }

            if (player.IsBot)
            {
                answer.Error = $"Player {playerId} is bot";
                return answer;
            }

            var now = DateTime.Today;
            bool flagCreateNewData;
            DateTime lastUpdateGameData;

            if (string.IsNullOrEmpty(player.LastUpdateGameData))
            {
                lastUpdateGameData = now;
                flagCreateNewData = true;
            }
            else
            {
                lastUpdateGameData = DateTime.Parse(player.LastUpdateGameData);
                flagCreateNewData = (lastUpdateGameData < now);
            }

            playerSave.PlayerInfoData = GetPlayerData(player);
            playerSave.City.MainCampaignSave = await _campaignController.GetPlayerSave(playerId);
            playerSave.City.MallSave = await _mallController.GetPlayerSave(playerId);
            playerSave.City.FortuneWheelData = await _fortuneWheelController.GetPlayerSave(playerId, flagCreateNewData);
            playerSave.City.TaskBoardData = await _taskBoardController.GetPlayerSave(playerId, flagCreateNewData);
            playerSave.City.DailyReward = await _dailyRewardController.GetPlayerSave(playerId, flagCreateNewData);

            playerSave.BattlepasData = await _battlepasController.GetPlayerSave(playerId, flagCreateNewData);

            playerSave.HeroesStorage = await GetHeroesSave(playerId);
            playerSave.InventoryData = await _inventoriesController.GetInventory(playerId);
            playerSave.Resources = await GetResourceSave(playerId);
            await _mailController.GetPlayerSave(playerId, playerSave.CommunicationData);
            //playerSave.City.TimeManagementSave = await _timeManagerController.GetPlayerSave(playerId);
            //playerSave.City.ChallengeTowerSave = await _challengeTowerController.GetPlayerSave(playerId);
            playerSave.AchievmentStorage = await _achievmentController.GetPlayerSave(playerId);
            playerSave.City.IndustrySave = await _industryController.GetPlayerSave(playerId, flagCreateNewData);
            playerSave.City.VoyageSave = await _voyageController.GetPlayerSave(playerId);
            playerSave.City.ArenaSave = await _arenaController.GetPlayerSave(playerId, playerSave.CommunicationData);
            playerSave.City.TravelCircleSave = await _travelCircleController.GetPlayerSave(playerId, flagCreateNewData);
            //playerSave.City.Tutorial = await _tutorialController.GetPlayerSave(playerId);
            playerSave.City.LongTravelData = await _longTravelController.GetPlayerSave(playerId, flagCreateNewData);
            playerSave.CycleEventsData = await _gameCycleController.GetPlayerSave(playerId);
            playerSave.AchievmentStorage = await _achievmentController.GetPlayerSave(playerId);
            await _friendshipController.GetPlayerSave(playerId, playerSave.CommunicationData);
            playerSave.City.GuildPlayerSaveContainer = await _guildController.GetPlayerSave(playerId, playerSave.CommunicationData, flagCreateNewData);

            var allTeams = await _context.ServerPlayerTeamDatas.ToListAsync();
            var playerTeams = allTeams.FindAll(team => team.PlayerId == playerId);
            foreach (var team in playerTeams)
            {
                playerSave.Teams.Add(team.Name, team.ArmyData);
            }
            if (flagCreateNewData)
            {
                player.LastUpdateGameData = now.ToString();
                await _context.SaveChangesAsync();
            }

            answer.Result = _jsonConverter.Serialize(playerSave);
            return answer;
        }

        private PlayerData GetPlayerData(Player player)
        {
            var result = new PlayerData(player);
            return result;
        }


        public async Task<HeroesStorage> GetHeroesSave(int playerId)
        {
            var result = new HeroesStorage();

            var heroes = await _context.Heroes.ToListAsync();

            var playerHeroes = heroes.FindAll(hero => hero.PlayerId == playerId);

            foreach (var hero in playerHeroes)
            {
                result.ListHeroes.Add(new HeroData(hero));
            }

            result.MaxCountHeroes = 100;

            return result;
        }

        public async Task<List<ResourceData>> GetResourceSave(int playerId)
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
