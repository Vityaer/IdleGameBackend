using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniverseRift.Contexts;
using UniverseRift.Controllers.Common;
using UniverseRift.GameModelDatas.Cities.TravelCircleRaces;
using UniverseRift.Models.Results;
using UniverseRift.Services.Rewarders;

namespace UniverseRift.Controllers.Buildings.TravelCircles
{
    public class TravelCircleController : ControllerBase, ITravelCircleController
    {
        private readonly AplicationContext _context;
        private readonly ICommonDictionaries _commonDictionaries;
        private readonly IRewardService _clientRewardService;

        public TravelCircleController(
            AplicationContext context,
            IRewardService clientRewardService,
            ICommonDictionaries commonDictionaries
            )
        {
            _context = context;
            _commonDictionaries = commonDictionaries;
            _clientRewardService = clientRewardService;
        }

        [HttpPost]
        [Route("TravelCircle/CompleteMission")]
        public async Task<AnswerModel> CompleteMission(int playerId, int travelId)
        {
            var answer = new AnswerModel();
            var tavelData = await _context.TravelRaceDatas.FindAsync(travelId);
            if (tavelData == null)
            {
                answer.Error = "Data error";
                return answer;
            }

            if (tavelData.PlayerId != playerId)
            {
                answer.Error = "Data error";
                return answer;
            }

            tavelData.MissionIndexCompleted += 1;

            var travelModel = _commonDictionaries.TravelRaceCampaigns[$"{tavelData.RaceId}Travel"];
            var mission = travelModel.Missions[tavelData.MissionIndexCompleted];
            await _clientRewardService.AddReward(playerId, mission.WinReward);

            await _context.SaveChangesAsync();
            answer.Result = "Success";
            return answer;
        }

        [HttpPost]
        [Route("TravelCircle/SmashMission")]
        public async Task<AnswerModel> SmashMission(int playerId, int travelId, int missionIndex, int count)
        {
            var answer = new AnswerModel();
            var tavelData = await _context.TravelRaceDatas.FindAsync(travelId);
            if (tavelData == null)
            {
                answer.Error = "Data error";
                return answer;
            }

            if (tavelData.PlayerId != playerId)
            {
                answer.Error = "Data error";
                return answer;
            }

            var travelModel = _commonDictionaries.TravelRaceCampaigns[$"{tavelData.RaceId}Travel"];
            if (missionIndex >= travelModel.Missions.Count || missionIndex < 0)
            {
                answer.Error = "Data error";
                return answer;
            }

            var mission = travelModel.Missions[missionIndex];
            for (var i = 0; i < count; i++)
            {
                await _clientRewardService.AddReward(playerId, mission.SmashReward);
            }

            await _context.SaveChangesAsync();
            answer.Result = "Success";
            return answer;
        }

        public async Task<TravelBuildingData> GetPlayerSave(int playerId, bool flagCreateNewData)
        {
            var result = new TravelBuildingData();
            var allTravelDatas = await _context.TravelRaceDatas.ToListAsync();
            var playerDatas = allTravelDatas.FindAll(data => data.PlayerId == playerId);

            result.TravelRaceDatas = playerDatas;

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
    }
}
