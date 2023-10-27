using Cysharp.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniRx;
using UniverseRift.Contexts;
using UniverseRift.Controllers.Common;
using UniverseRift.Controllers.Players.Heroes;
using UniverseRift.GameModelDatas.Cities;
using UniverseRift.GameModelDatas.Cities.TravelCircleRaces;
using UniverseRift.Models.Achievments;
using UniverseRift.Models.Results;
using UniverseRift.Services.Rewarders;

namespace UniverseRift.Controllers.Buildings.Achievments
{
    public class AchievmentController : ControllerBase, IAchievmentController, IDisposable
    {
        private const string DAILY_TASKS = "DailyTasks";

        private const string SIMPLE_HIRE_ACHIEVMENT_NAME = "DailySimpleHire";

        private readonly AplicationContext _context;
        private readonly ICommonDictionaries _commonDictionaries;
        private readonly IHeroesController _tavernController;
        private readonly CompositeDisposable _compositeDisposable = new();
        private readonly IRewardService _clientRewardService;

        public AchievmentController(
            AplicationContext context,
            ICommonDictionaries commonDictionaries,
            IHeroesController tavernController,
            IRewardService clientRewardService
            )
        {
            _context = context;
            _commonDictionaries = commonDictionaries;
            _tavernController = tavernController;
            _clientRewardService = clientRewardService;
        }

        public async Task<AchievmentStorageData> GetPlayerSave(int playerId)
        {
            var result = new AchievmentStorageData();
            var allAchievments = await _context.DailyTaskDatas.ToListAsync();
            var playerAchievments = allAchievments.FindAll(achievmentData => achievmentData.PlayerId == playerId);

            if (playerAchievments.Count == 0)
            {
                foreach (var dailyTaskId in _commonDictionaries.AchievmentContainers[DAILY_TASKS].TaskIds)
                {
                    var achievmentData = new AchievmentData(playerId, dailyTaskId);
                    playerAchievments.Add(achievmentData);
                }
                await _context.DailyTaskDatas.AddRangeAsync(playerAchievments);
            }

            result.Achievments = playerAchievments;
            return result;
        }

        public async Task OnRegistrationPlayer(int playerId)
        {
            var newRecords = new List<TravelRaceData>();

            foreach (var travel in _commonDictionaries.TravelRaceCampaigns.Values)
            {
                var newTravelData = new TravelRaceData(playerId, travel.Race);
                newRecords.Add(newTravelData);
            }

            await _context.TravelRaceDatas.AddRangeAsync(newRecords);
            await _context.SaveChangesAsync();
        }

        [HttpPost]
        [Route("Achievments/GetRewardAchievment")]
        public async Task<AnswerModel> GetRewardAchievment(int playerId, int achievmentId)
        {
            var answer = new AnswerModel();

            var dailyTask = await _context.DailyTaskDatas.FindAsync(achievmentId);
            if (dailyTask == null)
            {
                answer.Error = "Wrong data";
                return answer;
            }

            if (dailyTask.PlayerId != playerId)
            {
                answer.Error = "Wrong data";
                return answer;
            }

            var achievmentModel = _commonDictionaries.Achievments[dailyTask.ModelId];
            var rewardModel = achievmentModel.GetReward(dailyTask.CurrentStage);
            await _clientRewardService.AddReward(playerId, rewardModel);
            dailyTask.CurrentStage += 1;
            await _context.SaveChangesAsync();
            answer.Result = "Success";
            return answer;
        }

        public async Task ClearDailyTask()
        {
            await _context.DailyTaskDatas.ExecuteDeleteAsync();
        }

        public void Dispose()
        {
            _compositeDisposable.Dispose();
        }
    }
}
