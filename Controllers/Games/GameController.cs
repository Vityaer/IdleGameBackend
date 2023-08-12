using Microsoft.AspNetCore.Mvc;
using Misc.Json;
using UniverseRift.Controllers.Buildings.Achievments;
using UniverseRift.Controllers.Buildings.Arenas;
using UniverseRift.Controllers.Buildings.Campaigns;
using UniverseRift.Controllers.Buildings.ChallengeTowers;
using UniverseRift.Controllers.Buildings.DailyTasks;
using UniverseRift.Controllers.Buildings.FortuneWheels;
using UniverseRift.Controllers.Buildings.GameCycles;
using UniverseRift.Controllers.Buildings.Guilds;
using UniverseRift.Controllers.Buildings.Industries;
using UniverseRift.Controllers.Buildings.Shops;
using UniverseRift.Controllers.Buildings.TaskBoards;
using UniverseRift.Controllers.Buildings.TimeMenagers;
using UniverseRift.Controllers.Buildings.TravelCircles;
using UniverseRift.Controllers.Buildings.Tutorials;
using UniverseRift.Controllers.Buildings.Voyages;
using UniverseRift.Controllers.Players;
using UniverseRift.Controllers.Players.Heroes;
using UniverseRift.Controllers.Players.Inventories;
using UniverseRift.GameModelDatas.Players;
using UniverseRift.Models.Results;
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
        private readonly IDailyTasksController _dailyTasksController;
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
            IDailyTasksController dailyTasksController,
            IIndustryController industryController,
            IGameCycleController gameCycleController,
            IAchievmentController achievmentController,
            IVoyageController voyageController,
            IArenaController arenaController,
            ITravelCircleController travelCircleController,
            ITimeManagerController timeManagerController,
            ITutorialController tutorialController,
            IGuildController guildController,
            IFortuneWheelController fortuneWheelController
            )
        {
            _guildController = guildController;
            _tutorialController = tutorialController;
            _timeManagerController = timeManagerController;
            _travelCircleController = travelCircleController;
            _arenaController = arenaController;
            _voyageController = voyageController;
            _achievmentController = achievmentController;
            _gameCycleController = gameCycleController;
            _industryController = industryController;
            _dailyTasksController = dailyTasksController;
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
        }

        [HttpPost]
        [Route("GameController/GetPlayerSave")]
        public async Task<AnswerModel> GetPlayerSave(int playerId)
        {
            var answer = new AnswerModel();

            var playerSave = new CommonGameData();

            playerSave.PlayerInfoData = await _playersController.GetPlayerSave(playerId);
            
            playerSave.City.TimeManagementSave = await _timeManagerController.GetPlayerSave(playerId);
            playerSave.City.MainCampaignSave = await _campaignController.GetPlayerSave(playerId);
            playerSave.City.ChallengeTowerSave = await _challengeTowerController.GetPlayerSave(playerId);
            playerSave.City.MallSave = await _mallController.GetPlayerSave(playerId);
            playerSave.City.DailyTaskContainer = await _dailyTasksController.GetPlayerSave(playerId);
            playerSave.City.IndustrySave = await _industryController.GetPlayerSave(playerId);
            playerSave.City.VoyageSave = await _voyageController.GetPlayerSave(playerId);
            playerSave.City.ArenaSave = await _arenaController.GetPlayerSave(playerId);
            playerSave.City.TravelCircleSave = await _travelCircleController.GetPlayerSave(playerId);
            playerSave.City.Tutorial = await _tutorialController.GetPlayerSave(playerId);
            playerSave.City.GildSave = await _guildController.GetPlayerSave(playerId);
            playerSave.City.FortuneWheelData = await _fortuneWheelController.GetPlayerSave(playerId);
            playerSave.City.TaskBoardData = await _taskBoardController.GetPlayerSave(playerId);

            playerSave.CycleEventsData = await _gameCycleController.GetPlayerSave(playerId);
            playerSave.AchievmentStorage = await _achievmentController.GetPlayerSave(playerId);

            playerSave.HeroesStorage = await _heroesController.GetPlayerSave(playerId);
            playerSave.InventoryData = await _inventoriesController.GetInventory(playerId);
            playerSave.Resources = await _resourcesController.GetPlayerSave(playerId);
            answer.Result = _jsonConverter.Serialize(playerSave);
            return answer;
        }
    }
}
