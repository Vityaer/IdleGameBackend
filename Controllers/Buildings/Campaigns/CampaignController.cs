using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Misc.Json;
using UniRx;
using UniverseRift.Contexts;
using UniverseRift.Controllers.Common;
using UniverseRift.Controllers.Services.Rewarders;
using UniverseRift.Models.Players;
using UniverseRift.Models.Results;

namespace UniverseRift.Controllers.Buildings.Campaigns
{
    public class CampaignController : Controller, ICampaignController, IDisposable
    {
        private const int CHAPTER_MISSION_COUNT = 20;

        private readonly AplicationContext _context;
        private readonly ICommonDictionaries _commonDictionaries;
        private readonly IClientRewardService _clientRewardService;
        private readonly IJsonConverter _jsonConverter;
        
        private CompositeDisposable _disposables = new CompositeDisposable();
        public AplicationContext Context => _context;

        public CampaignController(
            AplicationContext context,
            ICommonDictionaries commonDictionaries,
            IClientRewardService clientRewardService,
            IJsonConverter jsonConverter
            )
        {
            _jsonConverter = jsonConverter;
            _clientRewardService = clientRewardService;
            _context = context;
            _commonDictionaries = commonDictionaries;
        }

        public async Task CreatePlayerProgress(int playerId)
        {
            var playerProgress = new PlayerProgress() { PlayerId = playerId, CampaignProgress = -1, ChellangeTowerProgress = -1, LastGetAutoFightReward = string.Empty };
            await _context.AddAsync(playerProgress);
            await _context.SaveChangesAsync();

            var players = await _context.PlayerProgresses.ToListAsync();
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

            var previousDateTime = string.IsNullOrEmpty(playerProgress.LastGetAutoFightReward)
                ?
                DateTime.Now
                :
                DateTime.Parse(playerProgress.LastGetAutoFightReward);

            var tact = CalculateCountTact(previousDateTime);
            playerProgress.LastGetAutoFightReward = DateTime.Now.ToString();
            var rewardData = autoReward.GetCaculateReward(tact, playerId);
            
            await _clientRewardService.AddReward(playerId, rewardData);

            await _context.SaveChangesAsync();

            answer.Result = _jsonConverter.ToJson(rewardData);
            return answer;
        }

        private int CalculateCountTact(DateTime previousDateTime, int MaxCount = 8640, int lenthTact = 5)
        {
            var localDate = DateTime.Now;
            var interval = localDate - previousDateTime;
            var tact = (int)interval.TotalSeconds / lenthTact;
            tact = Math.Min(tact, MaxCount);
            return tact;
        }

        [HttpPost]
        [Route("Campaign/CompleteNextMission")]
        public async Task<AnswerModel> CompleteNextMission(int playerId)
        {
            Console.WriteLine($"CompleteNextMission playerId: {playerId}");

            var answer = new AnswerModel();
            var result = await _context.Players.FindAsync(playerId);

            var playerProgresses = await _context.PlayerProgresses.ToListAsync();
            var resources = await _context.Resources.ToListAsync();

            var playerProgress = playerProgresses.Find(progress => progress.PlayerId == playerId);

            playerProgress.CampaignProgress += 1;
            if (string.IsNullOrEmpty(playerProgress.LastGetAutoFightReward))
                playerProgress.LastGetAutoFightReward = DateTime.Now.ToString();

            await _context.SaveChangesAsync();

            var numChapter = playerProgress.CampaignProgress / CHAPTER_MISSION_COUNT;
            var mission = _commonDictionaries.CampaignChapters.ElementAt(numChapter).Value.Missions[playerProgress.CampaignProgress % CHAPTER_MISSION_COUNT];
            await _clientRewardService.AddReward(playerId, mission.WinReward);


            answer.Result = playerProgress.CampaignProgress.ToString();
            return answer;
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
