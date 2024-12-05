using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Misc.Json;
using Models.Data.Inventories;
using UniverseRift.Contexts;
using UniverseRift.Controllers.Buildings.Achievments;
using UniverseRift.Controllers.Common;
using UniverseRift.GameModelDatas.Cities.Industries;
using UniverseRift.GameModels;
using UniverseRift.GameModels.Common;
using UniverseRift.Heplers.Utils;
using UniverseRift.Models.Resources;
using UniverseRift.Models.Results;
using UniverseRift.Services.Resources;
using UniverseRift.Services.Rewarders;

namespace UniverseRift.Controllers.Buildings.Industries.Mines
{
    public class MineController : ControllerBase, IMineController
    {
        private const string MAIN_MINE_NAME = "MainMineBuilding";
        private const string MAIN_MINE_PLACE_NAME = "MainMineBuildingPlace";

        private const int INCOME_SECONDS = 36000;
        private static Random _random;

        private readonly AplicationContext _context;
        private readonly ICommonDictionaries _commonDictionaries;
        private readonly IJsonConverter _jsonConverter;
        private readonly IResourceManager _resourceController;
        private readonly IRewardService _clientRewardService;
        private readonly IAchievmentController _achievmentController;


        public MineController(
            AplicationContext context,
            ICommonDictionaries commonDictionaries,
            IRewardService clientRewardService,
            IJsonConverter jsonConverter,
            IResourceManager resourceController,
            IAchievmentController achievmentController
            )
        {
            _context = context;
            _commonDictionaries = commonDictionaries;
            _jsonConverter = jsonConverter;
            _resourceController = resourceController;
            _clientRewardService = clientRewardService;
            _achievmentController = achievmentController;
        }

        [HttpPost]
        [Route("Mines/Create")]
        public async Task<AnswerModel> MineCreate(int playerId, string mineModelId, string placeId)
        {
            var answer = new AnswerModel();
            if (!_commonDictionaries.Mines.ContainsKey(mineModelId))
            {
                answer.Error = "Wrong data";
                return answer;
            }

            var mineModel = _commonDictionaries.Mines[mineModelId];

            var mineDatas = await _context.MineDatas.ToListAsync();
            var playerMineDatas = mineDatas.FindAll(data => data.PlayerId == playerId);

            var typeMineDatas = playerMineDatas.FindAll(data => data.MineId == mineModelId);
            var selectRestriction = _commonDictionaries.MineRestrictions.Values.ToList()
                .Find(restriction => restriction.MineId == mineModelId);

            var countMines = typeMineDatas.Count;

            if (ReferenceEquals(selectRestriction, null) || countMines == selectRestriction.MaxCount)
            {
                answer.Error = "Wrong data";
                return answer;
            }

            var cost = new List<Resource>();
            foreach (var resData in mineModel.CreateCost)
            {
                cost.Add(new Resource(playerId, new GameResource(resData)));
            }

            var enoughResource = await _resourceController.CheckResource(playerId, cost, answer);

            if (!enoughResource)
            {
                answer.Error = "Wrong data";
                return answer;
            }
            await _resourceController.SubstactResources(cost);

            var newMineData = new MineData(playerId, mineModelId, placeId);
            await _context.MineDatas.AddAsync(newMineData);

            await _context.SaveChangesAsync();
            answer.Result = _jsonConverter.Serialize(newMineData);
            return answer;
        }

        [HttpPost]
        [Route("Mines/LevelUp")]
        public async Task<AnswerModel> MineLevelUp(int playerId, int mineId)
        {
            var answer = new AnswerModel();

            var mineData = await _context.MineDatas.FindAsync(mineId);

            if (ReferenceEquals(mineData, null) || mineData.PlayerId != playerId)
            {
                answer.Error = "Wrong data";
                return answer;
            }

            var mineModel = _commonDictionaries.Mines[mineData.MineId];

            var cost = mineModel.CostLevelUpContainer.GetCostForLevelUp(mineData.Level + 1, playerId);

            var enoughResource = await _resourceController.CheckResource(playerId, cost, answer);

            if (!enoughResource)
            {
                answer.Error = "Wrong data";
                return answer;
            }
            await _resourceController.SubstactResources(cost);

            mineData.Level += 1;
            await _context.SaveChangesAsync();

            answer.Result = "Success";
            return answer;
        }

        [HttpPost]
        [Route("Mines/TakeMineResources")]
        public async Task<AnswerModel> TakeMineResources(int playerId, int mineId)
        {
            var answer = new AnswerModel();

            var mineData = await _context.MineDatas.FindAsync(mineId);

            if (ReferenceEquals(mineData, null) || mineData.PlayerId != playerId)
            {
                answer.Error = "Wrong data";
                return answer;
            }

            var startDateTime = DateTimeUtils.TryParseOrNow(mineData.LastDateTimeGetIncome);

            var currentTimeSpan = DateTime.UtcNow - startDateTime;

            if (currentTimeSpan.TotalSeconds > INCOME_SECONDS)
                currentTimeSpan = new TimeSpan(0, 0, INCOME_SECONDS);

            var timeFactor = (float)(currentTimeSpan.TotalSeconds / INCOME_SECONDS);

            var mineModel = _commonDictionaries.Mines[mineData.MineId];
            var income = mineModel.IncomesContainer.GetCostForLevelUp(mineData.Level, playerId)[0];
            var currentResource = income * timeFactor;

            var rewardModel = new RewardModel();
            rewardModel.Add(new ResourceData() { Type = income.Type, Amount = new BigDigit(income.Count, income.E10) });

            await _clientRewardService.AddReward(playerId, rewardModel);

            mineData.LastDateTimeGetIncome = DateTime.UtcNow.ToString(Constants.Common.DateTimeFormat);

            await _context.SaveChangesAsync();

            answer.Result = _jsonConverter.Serialize(rewardModel);
            return answer;
        }

        [HttpPost]
        [Route("Mines/DestroyMine")]
        public async Task<AnswerModel> DestroyMine(int playerId, int mineId)
        {
            var answer = new AnswerModel();

            var mineData = await _context.MineDatas.FindAsync(mineId);

            if (ReferenceEquals(mineData, null) || mineData.PlayerId != playerId || mineData.MineId.Equals(MAIN_MINE_NAME))
            {
                answer.Error = "Wrong data";
                return answer;
            }

            _context.MineDatas.Remove(mineData);
            await _context.SaveChangesAsync();

            answer.Result = "Success";
            return answer;
        }

        [HttpPost]
        [Route("Mines/TakeAllResources")]
        public async Task<AnswerModel> TakeAllResources(int playerId)
        {
            var answer = new AnswerModel();

            var mineDatas = await _context.MineDatas.ToListAsync();

            var playerMineDatas = mineDatas.FindAll(data => data.PlayerId == playerId);
            var rewardModel = new RewardModel();

            foreach (var mineData in playerMineDatas)
            {
                if (mineData.PlayerId != playerId)
                {
                    answer.Error = "Wrong data";
                    return answer;
                }

                var startDateTime = DateTimeUtils.TryParseOrNow(mineData.LastDateTimeGetIncome);

                var currentTimeSpan = DateTime.UtcNow - startDateTime;

                if (currentTimeSpan.TotalSeconds > INCOME_SECONDS)
                    currentTimeSpan = new TimeSpan(0, 0, INCOME_SECONDS);

                var timeFactor = (float)(currentTimeSpan.TotalSeconds / INCOME_SECONDS);

                var mineModel = _commonDictionaries.Mines[mineData.MineId];
                var income = mineModel.IncomesContainer.GetCostForLevelUp(mineData.Level, playerId)[0];
                var currentResource = income * timeFactor;

                rewardModel.Add(new ResourceData() { Type = income.Type, Amount = new BigDigit(income.Count, income.E10) });
                mineData.LastDateTimeGetIncome = DateTime.UtcNow.ToString(Constants.Common.DateTimeFormat);
            }

            await _clientRewardService.AddReward(playerId, rewardModel);

            await _context.SaveChangesAsync();
            await _achievmentController.AchievmentUpdataData(playerId, "MineGetResourceCountAchievment", 1);


            answer.Result = _jsonConverter.Serialize(rewardModel);
            return answer;
        }

        [HttpPost]
        [Route("Mines/FinishMission")]
        public async Task<AnswerModel> FinishMission(int playerId, int missionId)
        {
            var answer = new AnswerModel();

            var mineMissionData = await _context.MineMissionDatas.FindAsync(missionId);

            if (ReferenceEquals(mineMissionData, null) || mineMissionData.PlayerId != playerId)
            {
                answer.Error = "Not found mine mission for this player.";
                return answer;
            }

            if (mineMissionData.IsComplete)
            {
                answer.Error = "This mission was completed.";
                return answer;
            }

            var travelContainer = _commonDictionaries.StorageChallenges[mineMissionData.StorageMissionContainerId];
            var missionModel = travelContainer.Missions.Find(mission => mission.Name.Equals(mineMissionData.MissionId));
            if (ReferenceEquals(missionModel, null))
            {
                answer.Error = "Server error with mine mission model.";
                return answer;
            }

            await _clientRewardService.AddReward(playerId, missionModel.WinReward);
            mineMissionData.IsComplete = true;

            await _context.SaveChangesAsync();

            answer.Result = "Success";
            return answer;
        }

        public async Task OnRegistrationPlayer(int playerId)
        {
            var newMineData = new MineData(playerId, MAIN_MINE_NAME, MAIN_MINE_PLACE_NAME);
            await _context.MineDatas.AddAsync(newMineData);

            var travelLevel = newMineData.Level / 3;
            var countTravelOpen = Math.Clamp(newMineData.Level / 2, 5, 15);

            var tavelName = $"MineTravelLevel_{travelLevel}";
            var travel = _commonDictionaries.StorageChallenges[tavelName];
            var mineMissions = new List<MineMissionData>(countTravelOpen);

            if (_random == null)
                _random = new Random();

            for (var i = 0; i < countTravelOpen; i++)
            {
                var randIndex = _random.Next(travel.Missions.Count);
                var mission = travel.Missions[randIndex];
                var newMineMission = new MineMissionData(playerId, tavelName, mission);
                mineMissions.Add(newMineMission);
            }

            await _context.MineMissionDatas.AddRangeAsync(mineMissions);

            await _context.SaveChangesAsync();
        }
    }
}
