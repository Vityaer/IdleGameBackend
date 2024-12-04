using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniverseRift.Contexts;
using UniverseRift.Controllers.Buildings.Achievments;
using UniverseRift.Controllers.Common;
using UniverseRift.GameModelDatas.Cities;
using UniverseRift.Models.Results;
using UniverseRift.Services.Rewarders;

namespace UniverseRift.Controllers.Buildings.ChallengeTowers
{
    public class ChallengeTowerController : ControllerBase, IChallengeTowerController
    {
        private readonly AplicationContext _context;
        private readonly ICommonDictionaries _commonDictionaries;
        private readonly IRewardService _clientRewardService;
        private readonly IAchievmentController _achievmentController;

        public ChallengeTowerController(
            AplicationContext context,
            ICommonDictionaries commonDictionaries,
            IRewardService clientRewardService,
            IAchievmentController achievmentController
            )
        {
            _context = context;
            _commonDictionaries = commonDictionaries;
            _clientRewardService = clientRewardService;
            _achievmentController = achievmentController;
        }

        public async Task<BuildingWithFightTeamsData> GetPlayerSave(int playerId)
        {
            var result = new BuildingWithFightTeamsData();
            return result;
        }

        [HttpPost]
        [Route("ChallengeTower/CompleteNextMission")]
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

            playerProgress.ChellangeTowerProgress += 1;
            var mission = _commonDictionaries.StorageChallenges["ChallengeTower"]
                .Missions[playerProgress.ChellangeTowerProgress];

            await _clientRewardService.AddReward(playerId, mission.WinReward);

            await _context.SaveChangesAsync();
            await _achievmentController.AchievmentUpdataData(playerId, "CompleteChallengeTowerMissionAchievment", 1);

            answer.Result = "Success!";
            return answer;
        }

        [HttpPost]
        [Route("ChallengeTower/TryMission")]
        public async Task<AnswerModel> TryMission(int playerId)
        {
            var answer = new AnswerModel();

            var playerProgresses = await _context.PlayerProgresses.ToListAsync();

            var playerProgress = playerProgresses.Find(progress => progress.PlayerId == playerId);

            if (playerProgress == null)
            {
                answer.Error = "Progress not found.";
                return answer;
            }

            await _achievmentController.AchievmentUpdataData(playerId, "TryDeathTowerMissionAchievment", 1);


            await _context.SaveChangesAsync();

            answer.Result = "Success!";
            return answer;
        }
    }
}
