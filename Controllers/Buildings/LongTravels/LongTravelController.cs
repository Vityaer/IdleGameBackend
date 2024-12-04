using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniverseRift.Contexts;
using UniverseRift.Controllers.Buildings.Achievments;
using UniverseRift.Controllers.Common;
using UniverseRift.Models.LongTravels;
using UniverseRift.Models.Results;
using UniverseRift.Services.Rewarders;

namespace UniverseRift.Controllers.Buildings.LongTravels
{
    public class LongTravelController : ControllerBase, ILongTravelController
    {
        private readonly AplicationContext _context;
        private readonly ICommonDictionaries _commonDictionaries;
        private readonly IRewardService _clientRewardService;
        private readonly IAchievmentController _achievmentController;

        public LongTravelController(
            AplicationContext context,
            ICommonDictionaries commonDictionaries,
            IRewardService clientRewardService,
            IAchievmentController achievmentController
            )
        {
            _context = context;
            _clientRewardService = clientRewardService;
            _commonDictionaries = commonDictionaries;
            _achievmentController = achievmentController;
        }

        public async Task<LongTravelData> GetPlayerSave(int playerId, bool flagCreateNewData)
        {
            var result = new LongTravelData();
            var serverDatas = await _context.LongTravelServerDatas.ToListAsync();

            var playerServerData = serverDatas.Find(data => data.PlayerId == playerId);
            if (playerServerData == null)
                return result;

            if (flagCreateNewData)
            {
                playerServerData.MainTravelAmount = 0;
                playerServerData.HeroTravelAmount = 0;
                playerServerData.TrainTravelAmount = 0;
                await _context.SaveChangesAsync();
            }

            result.TravelProgress.Add(LongTravelType.Main, playerServerData.MainTravelAmount);
            result.TravelProgress.Add(LongTravelType.Hero, playerServerData.HeroTravelAmount);
            result.TravelProgress.Add(LongTravelType.Train, playerServerData.TrainTravelAmount);

            return result;
        }

        public async Task OnPlayerRegister(int playerId)
        {
            var longTravelsData = new LongTravelServerData() { PlayerId = playerId };
            await _context.LongTravelServerDatas.AddAsync(longTravelsData);
            await _context.SaveChangesAsync();
        }

        [HttpPost]
        [Route("LongTravel/MissionComplete")]
        public async Task<AnswerModel> MissionComplete(int playerId, int travelType, int missionIndex, int result)
        {
            var answer = new AnswerModel();
            var player = await _context.Players.FindAsync(playerId);

            if (player == null)
            {
                answer.Error = "Not found player";
                return answer;
            }
            var serverDatas = await _context.LongTravelServerDatas.ToListAsync();

            var playerServerData = serverDatas.Find(data => data.PlayerId == playerId);
            if (playerServerData == null)
            {
                answer.Error = "Not found long travel data";
                return answer;
            }

            var type = (LongTravelType)travelType;
            switch (type)
            {
                case LongTravelType.Main:
                    playerServerData.MainTravelAmount += 1;
                    break;
                case LongTravelType.Train:
                    playerServerData.TrainTravelAmount += 1;
                    break;
                case LongTravelType.Hero:
                    playerServerData.HeroTravelAmount += 1;
                    break;
            }

            if (result == 1)
            {
                var currentMissionContainer = _commonDictionaries.StorageChallenges[$"LongTravel_{type}"];
                var missionModel = currentMissionContainer.Missions[missionIndex];
                await _clientRewardService.AddReward(playerId, missionModel.WinReward);
            }

            await _achievmentController.AchievmentUpdataData(playerId, "LongTravelTryCountAchievment", 1);
            await _context.SaveChangesAsync();

            answer.Result = "Success";
            return answer;
        }
    }
}
