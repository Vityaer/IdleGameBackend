using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Misc.Json;
using System.Globalization;
using UniRx;
using UniverseRift.Contexts;
using UniverseRift.Controllers.Buildings.Achievments;
using UniverseRift.Controllers.Common;
using UniverseRift.GameModelDatas.Cities;
using UniverseRift.GameModelDatas.Cities.Industries;
using UniverseRift.Heplers.Utils;
using UniverseRift.Models.Players;
using UniverseRift.Models.Results;
using UniverseRift.Services.Rewarders;

namespace UniverseRift.Controllers.Buildings.Campaigns
{
    public class CampaignController : Controller, ICampaignController, IDisposable
    {
        private const string NAME_RECORD_NUM_CURRENT_MISSION = "CurrentMission";
        private const string NAME_RECORD_NUM_MAX_MISSION = "MaxMission";
        private const string NAME_RECORD_AUTOFIGHT_PREVIOUS_DATETIME = "AutoFight";

        private const int CHAPTER_MISSION_COUNT = 20;

        private readonly AplicationContext _context;
        private readonly ICommonDictionaries _commonDictionaries;
        private readonly IRewardService _clientRewardService;
        private readonly IJsonConverter _jsonConverter;
        private readonly IAchievmentController _achievmentController;

        private CompositeDisposable _disposables = new CompositeDisposable();

        public CampaignController(
            AplicationContext context,
            ICommonDictionaries commonDictionaries,
            IRewardService clientRewardService,
            IJsonConverter jsonConverter,
            IAchievmentController achievmentController
            )
        {
            _context = context;
            _jsonConverter = jsonConverter;
            _commonDictionaries = commonDictionaries;
            _clientRewardService = clientRewardService;
            _achievmentController = achievmentController;
        }

        public async Task CreatePlayerProgress(int playerId)
        {
            var playerProgress = new PlayerProgress() { PlayerId = playerId, CampaignProgress = 0, ChellangeTowerProgress = 0, LastGetAutoFightReward = string.Empty };
            await _context.AddAsync(playerProgress);
            await _context.SaveChangesAsync();
        }

        [HttpPost]
        [Route("Campaign/GetAutoFightReward")]
        public async Task<AnswerModel> GetAutoFightReward(int playerId, int numMission)
        {
            var answer = new AnswerModel();

            var playerProgresses = await _context.PlayerProgresses.ToListAsync();

            var playerProgress = playerProgresses.Find(progress => progress.PlayerId == playerId);
            if (numMission > playerProgress.CampaignProgress)
            {
                answer.Error = "Player not complete this mission";
                return answer;
            }

            var numChapter = numMission / CHAPTER_MISSION_COUNT;
            var mission = _commonDictionaries.CampaignChapters.ElementAt(numChapter).Value.Missions[numMission % CHAPTER_MISSION_COUNT];
            var autoReward = mission.AutoFightReward;

            var previousDateTime = DateTimeUtils.TryParseOrNow(playerProgress.LastGetAutoFightReward);

            var tact = CalculateCountTact(previousDateTime);

            var interval = DateTime.UtcNow - previousDateTime;
            await _achievmentController.AchievmentUpdataData(playerId, "AutoFightHoursAchievment", (int)interval.TotalHours);

            playerProgress.LastGetAutoFightReward = DateTime.UtcNow.ToString(Constants.Common.DateTimeFormat);
            var rewardModel = autoReward.GetCaculateReward(tact, playerId);
            
            await _clientRewardService.AddReward(playerId, rewardModel);

            await _context.SaveChangesAsync();

            answer.Result = _jsonConverter.Serialize(rewardModel);
            return answer;
        }

        private int CalculateCountTact(DateTime previousDateTime, int MaxCount = 8640, int lenthTact = 5)
        {
            var localDate = DateTime.UtcNow;
            var interval = localDate - previousDateTime;
            var tact = (int)interval.TotalSeconds / lenthTact;
            tact = Math.Min(tact, MaxCount);
            return tact;
        }

        [HttpPost]
        [Route("Campaign/CompleteNextMission")]
        public async Task<AnswerModel> CompleteNextMission(int playerId)
        {
            var answer = new AnswerModel();
            var result = await _context.Players.FindAsync(playerId);

            var playerProgresses = await _context.PlayerProgresses.ToListAsync();
            var resources = await _context.Resources.ToListAsync();

            var playerProgress = playerProgresses.Find(progress => progress.PlayerId == playerId);

            if (playerProgress == null)
            {
                answer.Error = "Progress not found.";
                return answer;
            }

            playerProgress.CampaignProgress += 1;
            if (string.IsNullOrEmpty(playerProgress.LastGetAutoFightReward))
                playerProgress.LastGetAutoFightReward = DateTime.UtcNow.ToString(Constants.Common.DateTimeFormat);

            await _context.SaveChangesAsync();

            var numChapter = playerProgress.CampaignProgress / CHAPTER_MISSION_COUNT;
            var mission = _commonDictionaries.CampaignChapters.ElementAt(numChapter).Value.Missions[playerProgress.CampaignProgress % CHAPTER_MISSION_COUNT];
            await _clientRewardService.AddReward(playerId, mission.WinReward);

            answer.Result = playerProgress.CampaignProgress.ToString();
            return answer;
        }

        public async Task<MainCampaignBuildingData> GetPlayerSave(int playerId)
        {
            var playerProgresses = await _context.PlayerProgresses.ToListAsync();
            var playerProgress = playerProgresses.Find(progress => progress.PlayerId == playerId);

            var result = new MainCampaignBuildingData();
            
            if (playerProgress != null)
            {
                result.IntRecords.SetRecord(NAME_RECORD_NUM_MAX_MISSION, playerProgress.CampaignProgress);
                var date = string.IsNullOrEmpty(playerProgress.LastGetAutoFightReward) ? string.Empty : playerProgress.LastGetAutoFightReward;
                result.LastGetAutoFightReward = date;
            }

            return result;
        }

        public new void Dispose()
        {
            _disposables.Dispose();
            base.Dispose();
        }
    }
}
