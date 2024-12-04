using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniverseRift.Contexts;
using UniverseRift.Controllers.Common;
using UniverseRift.Models.City.DailyRewards;
using UniverseRift.Models.Results;
using UniverseRift.Services.Rewarders;

namespace UniverseRift.Controllers.Buildings.DailyRewards
{
    public class DailyRewardController : ControllerBase, IDailyRewardController
    {
        private const string DAILY_REWARDS = "DailyRewards";

        private readonly AplicationContext _context;
        private readonly ICommonDictionaries _commonDictionaries;
        private readonly IRewardService _clientRewardService;

        public DailyRewardController(
            AplicationContext context,
            ICommonDictionaries commonDictionaries,
            IRewardService clientRewardService
            )
        {
            _context = context;
            _clientRewardService = clientRewardService;
            _commonDictionaries = commonDictionaries;
        }

        public async Task<DailyRewardContainer> GetPlayerSave(int playerId, bool flagCreateNewData)
        {
            var dailyRewardSaves = await _context.DailyRewardProgresses.ToListAsync();
            var playerData = dailyRewardSaves.Find(save => save.PlayerId == playerId);

            var needSave = false;
            if (playerData == null)
            {
                playerData = new DailyRewardProgress();
                playerData.PlayerId = playerId;
                await _context.DailyRewardProgresses.AddAsync(playerData);
                needSave = true;
            }

            var result = new DailyRewardContainer();
            result.CanGetDailyReward = flagCreateNewData;
            result.CurrentDailyReward = playerData.ReceivedIndex;

            if (flagCreateNewData)
            {
                playerData.CanGetDailyReward = true;
                needSave = true;
            }

            if(needSave)
                await _context.SaveChangesAsync();

            return result;
        }

        [HttpPost]
        [Route("DailyReward/GetNextReward")]
        public async Task<AnswerModel> GetNextReward(int playerId)
        {
            var answer = new AnswerModel();
            var dailyRewardSaves = await _context.DailyRewardProgresses.ToListAsync();
            var battlepas = dailyRewardSaves.Find(save => save.PlayerId == playerId);

            if (battlepas == null)
            {
                answer.Error = "Wrong data, battlepas null";
                return answer;
            }

            var nextIndex = battlepas.ReceivedIndex + 1;
            var rewards = _commonDictionaries.RewardContainerModels[DAILY_REWARDS].Rewards;
            if (nextIndex >= rewards.Count || !battlepas.CanGetDailyReward)
            {
                answer.Error = "Wrong data, next index wrong";
                return answer;
            }

            await _clientRewardService.AddReward(playerId, rewards[nextIndex]);

            battlepas.ReceivedIndex = nextIndex;
            battlepas.CanGetDailyReward = false;

            await _context.SaveChangesAsync();
            answer.Result = "Success";
            return answer;
        }

        //private string CreateRewards()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
