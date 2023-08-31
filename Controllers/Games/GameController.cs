using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Misc.Json;
using Models.Common.BigDigits;
using Models.Data.Inventories;
using System;
using UniverseRift.Contexts;
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
using UniverseRift.Controllers.Common;
using UniverseRift.Controllers.Players;
using UniverseRift.Controllers.Players.Heroes;
using UniverseRift.Controllers.Players.Inventories;
using UniverseRift.Controllers.Server;
using UniverseRift.GameModelDatas.Cities;
using UniverseRift.GameModelDatas.Players;
using UniverseRift.MessageData;
using UniverseRift.Models.City.FortuneWheels;
using UniverseRift.Models.City.Markets;
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
        private readonly IServerController _serverController;
        private readonly ICommonDictionaries _commonDictionaries;
        private readonly AplicationContext _context;

        private bool _isCreated = false;
        private readonly Random _random = new Random();

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
            IFortuneWheelController fortuneWheelController,
            ICommonDictionaries commonDictionaries,
            IServerController serverController,
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
            _serverController = serverController;
            _commonDictionaries = commonDictionaries;
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
            playerSave.City.MainCampaignSave = await GetCampaignSave(playerId);
            playerSave.City.MallSave = await GetMarketSave(playerId);
            playerSave.City.FortuneWheelData = await GetFortuneSave(playerId, flagCreateNewData);
            playerSave.City.TaskBoardData = await GetTaskboardSave(playerId, flagCreateNewData);
            
            playerSave.HeroesStorage = await GetHeroesSave(playerId);
            playerSave.InventoryData = await GetInventory(playerId);
            playerSave.Resources = await GetResourceSave(playerId);

            //playerSave.City.TimeManagementSave = await _timeManagerController.GetPlayerSave(playerId);
            //playerSave.City.ChallengeTowerSave = await _challengeTowerController.GetPlayerSave(playerId);
            //playerSave.City.DailyTaskContainer = await _dailyTasksController.GetPlayerSave(playerId);
            //playerSave.City.IndustrySave = await _industryController.GetPlayerSave(playerId);
            //playerSave.City.VoyageSave = await _voyageController.GetPlayerSave(playerId);
            //playerSave.City.ArenaSave = await _arenaController.GetPlayerSave(playerId);
            //playerSave.City.TravelCircleSave = await _travelCircleController.GetPlayerSave(playerId, flagCreateNewData);
            //playerSave.City.Tutorial = await _tutorialController.GetPlayerSave(playerId);
            //playerSave.City.GildSave = await _guildController.GetPlayerSave(playerId);

            //playerSave.CycleEventsData = await _gameCycleController.GetPlayerSave(playerId);
            //playerSave.AchievmentStorage = await _achievmentController.GetPlayerSave(playerId);


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

        private const string NAME_RECORD_NUM_MAX_MISSION = "MaxMission";
        private const string NAME_RECORD_AUTOFIGHT_PREVIOUS_DATETIME = "AutoFight";
        public async Task<BuildingWithFightTeamsData> GetCampaignSave(int playerId)
        {
            var playerProgresses = await _context.PlayerProgresses.ToListAsync();
            var playerProgress = playerProgresses.Find(progress => progress.PlayerId == playerId);

            var result = new BuildingWithFightTeamsData();

            if (playerProgress != null)
            {
                result.IntRecords.SetRecord(NAME_RECORD_NUM_MAX_MISSION, playerProgress.CampaignProgress);
                var date = string.IsNullOrEmpty(playerProgress.LastGetAutoFightReward) ? DateTime.UtcNow : DateTime.Parse(playerProgress.LastGetAutoFightReward);
                result.DateRecords.SetRecord(NAME_RECORD_AUTOFIGHT_PREVIOUS_DATETIME, date);
            }

            return result;
        }

        public async Task<MarketData> GetMarketSave(int playerId)
        {
            var result = new MarketData();
            var purchases = await _context.Purchases.ToListAsync();
            var playerPurchases = purchases.FindAll(purchase => purchase.PlayerId == playerId);

            result.PurchaseDatas = playerPurchases
                                    .Select(purchase =>
                                        new PurchaseData
                                        {
                                            ProductId = purchase.ProductId,
                                            PurchaseCount = purchase.PurchaseCount
                                        }
                                     ).ToList();
            return result;
        }

        public async Task<FortuneWheelData> GetFortuneSave(int playerId, bool flagCreateNewData)
        {
            var playerWheel = await LoadFortuneWheel(playerId);

            if (playerWheel == null || flagCreateNewData)
            {
                playerWheel = await CreateFortuneWheel(playerId);
            }

            var result = _jsonConverter.Deserialize<FortuneWheelData>(playerWheel.RewardsJson);

            return result;
        }

        private const int REWARD_COUNT = 8;
        private async Task<FortuneWheelModel> CreateFortuneWheel(int playerId)
        {
            var playerWheel = await LoadFortuneWheel(playerId);
            var newWheel = new FortuneWheelModel();

            var rewardsData = new FortuneWheelData();

            var rewardModels = _commonDictionaries.FortuneRewardModels.ToList();
            for (var i = 0; i < REWARD_COUNT; i++)
            {
                var rand = _random.Next(0, rewardModels.Count);
                var randomReward = rewardModels[rand].Value;
                rewardsData.Rewards.Add(new FortuneRewardData { RewardModelId = randomReward.Id });
            }
            newWheel.PlayerId = playerId;
            newWheel.RewardsJson = _jsonConverter.Serialize(rewardsData);

            if (playerWheel == null)
            {
                await _context.FortuneWheels.AddAsync(newWheel);
            }

            playerWheel = newWheel;
            await _context.SaveChangesAsync();

            return playerWheel;
        }

        private async Task<FortuneWheelModel> LoadFortuneWheel(int playerId)
        {
            var wheels = await _context.FortuneWheels.ToListAsync();
            var playerWheel = wheels.Find(wheel => wheel.PlayerId == playerId);
            return playerWheel;
        }

        private const int DAILY_TASK_COUNT = 5;
        public async Task<TaskBoardData> GetTaskboardSave(int playerId, bool flagCreateNewData)
        {
            var result = new TaskBoardData();
            var allTasks = await _context.GameTasks.ToListAsync();

            var playerTasks = allTasks.FindAll(task => task.PlayerId == playerId);

            if (flagCreateNewData)
            {
                var notStartableTasks = playerTasks.FindAll(task => task.Status == TaskStatusType.NotStart);

                if (notStartableTasks.Count < DAILY_TASK_COUNT)
                {
                    var notEnoughCount = DAILY_TASK_COUNT - notStartableTasks.Count;
                    GetNewTasks(playerId, notEnoughCount, out var newTasks);
                    playerTasks.AddRange(newTasks);

                    await _context.GameTasks.AddRangeAsync(newTasks);
                    await _context.SaveChangesAsync();
                }
            }

            foreach (var task in playerTasks)
            {
                result.ListTasks.Add(new TaskData(task));
            }

            return result;
        }

        private void GetNewTasks(int playerId, int count, out List<GameTask> resultTasks)
        {
            var gameTaskModels = _commonDictionaries.GameTaskModels.ToList();

            resultTasks = new List<GameTask>(count);

            for (var i = 0; i < count; i++)
            {
                var random = _random.Next(0, gameTaskModels.Count);
                var taskModel = gameTaskModels[random].Value;

                var intFactorDelta = (int)(taskModel.FactorDelta * 100f);
                var randFactor = _random.Next(100 - intFactorDelta, 100 + intFactorDelta + 1) / 100f;
                randFactor = (float)Math.Round(randFactor, 2);

                var newTask = new GameTask(playerId, taskModel, randFactor);
                resultTasks.Add(newTask);
            }
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

        public async Task<InventoryData> GetInventory(int playerId)
        {
            var result = new InventoryData();
            var items = await _context.Items.ToListAsync();
            var playerItems = items.FindAll(item => item.PlayerId == playerId);

            foreach (var item in playerItems)
            {
                result.Items.Add(new ItemData { Id = item.Name, Amount = (int)Math.Round(item.Count) });
            }

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
