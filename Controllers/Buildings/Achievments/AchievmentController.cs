using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniRx;
using UniverseRift.Contexts;
using UniverseRift.Controllers.Common;
using UniverseRift.GameModelDatas.Cities;
using UniverseRift.GameModels.Common;
using UniverseRift.Models.Achievments;
using UniverseRift.Models.Results;
using UniverseRift.Services.Rewarders;

namespace UniverseRift.Controllers.Buildings.Achievments
{
    public class AchievmentController : ControllerBase, IAchievmentController, IDisposable
    {
        private const string DAILY_TASKS = "DailyTasks";

        private readonly AplicationContext _context;
        private readonly ICommonDictionaries _commonDictionaries;
        private readonly CompositeDisposable _compositeDisposable = new();
        private readonly IRewardService _clientRewardService;

        public AchievmentController(
            AplicationContext context,
            ICommonDictionaries commonDictionaries,
            IRewardService clientRewardService
            )
        {
            _context = context;
            _commonDictionaries = commonDictionaries;
            _clientRewardService = clientRewardService;
        }

        public async Task<AchievmentStorageData> GetPlayerSave(int playerId)
        {
            var result = new AchievmentStorageData();
            var allAchievments = await _context.AchievmentDatas.ToListAsync();
            var playerAchievments = allAchievments.FindAll(achievmentData => achievmentData.PlayerId == playerId);

            if (playerAchievments.Count == 0)
            {
                foreach (var container in _commonDictionaries.AchievmentContainers.Values)
                {
                    foreach (var dailyTaskId in container.TaskIds)
                    {
                        var achievmentData = new AchievmentData(playerId, dailyTaskId);
                        playerAchievments.Add(achievmentData);
                    }
                }
                await _context.AchievmentDatas.AddRangeAsync(playerAchievments);
            }

            result.Achievments = playerAchievments;
            return result;
        }

        public async Task OnRegistrationPlayer(int playerId)
        {
            var playerAchievments = new List<AchievmentData>();

            foreach (var container in _commonDictionaries.AchievmentContainers.Values)
            {
                foreach (var dailyTaskId in container.TaskIds)
                {
                    var achievmentData = new AchievmentData(playerId, dailyTaskId);
                    playerAchievments.Add(achievmentData);
                }
            }
            await _context.AchievmentDatas.AddRangeAsync(playerAchievments);
            await _context.SaveChangesAsync();
        }

        [HttpPost]
        [Route("Achievments/GetRewardAchievment")]
        public async Task<AnswerModel> GetRewardAchievment(int playerId, int achievmentId)
        {
            var answer = new AnswerModel();

            var dailyTask = await _context.AchievmentDatas.FindAsync(achievmentId);
            if (dailyTask == null)
            {
                answer.Error = "Achievment not found!";
                return answer;
            }

            if (dailyTask.PlayerId != playerId)
            {
                answer.Error = "Wrong player data";
                return answer;
            }

            var achievmentModel = _commonDictionaries.Achievments[dailyTask.ModelId];

            if (achievmentModel.Stages.Count == dailyTask.CurrentStage || dailyTask.IsComplete)
            {
                answer.Error = "Achievment was done early!";
                return answer;
            }

            var currentAmount = new BigDigit(dailyTask.Amount, dailyTask.E10);
            var requireAmount = achievmentModel.Stages[dailyTask.CurrentStage].RequireCount;

            if (!currentAmount.CheckCount(requireAmount))
            {
                answer.Error = "Not enough amount for reward.";
                return answer;
            }

            var rewardModel = achievmentModel.GetReward(dailyTask.CurrentStage);
            await _clientRewardService.AddReward(playerId, rewardModel);
            dailyTask.CurrentStage += 1;
            if (dailyTask.CurrentStage == achievmentModel.Stages.Count)
                dailyTask.IsComplete = true;

            await _context.SaveChangesAsync();
            answer.Result = "Success";
            return answer;
        }

        public async Task AchievmentUpdataData(int playerId, string implementationName, int amount)
        {
            var allAchievments = await _context.AchievmentDatas.ToListAsync();
            var playerAchievments = allAchievments
                .FindAll(achievmentData => achievmentData.PlayerId == playerId);

            var models = _commonDictionaries.Achievments
                .Where(data => data.Value.ImplementationName.Equals(implementationName))
                .Select(data => data.Value);
            foreach (var achievmentModel in models)
            {
                var achievment = allAchievments.Find(
                    achievment => achievment.PlayerId == playerId
                    && achievment.ModelId == achievmentModel.Id);

                if (achievment == null)
                {
                    achievment = new AchievmentData(playerId, achievmentModel.Id);
                    await _context.AchievmentDatas.AddAsync(achievment);
                }
                achievment.Amount += amount;
            }

            await _context.SaveChangesAsync();
        }

        public async Task RefreshDailyTask()
        {
            var allAchievments = await _context.AchievmentDatas.ToListAsync();
            foreach (var achievmentData in allAchievments)
            {
                foreach (var achievmentId in _commonDictionaries.AchievmentContainers[DAILY_TASKS].TaskIds)
                {
                    if (achievmentData.ModelId.Equals(achievmentId))
                    {
                        achievmentData.Refresh();
                        break;
                    }
                }
            }

            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _compositeDisposable.Dispose();
        }
    }
}
