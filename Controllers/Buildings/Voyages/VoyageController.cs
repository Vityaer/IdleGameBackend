using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniverseRift.Contexts;
using UniverseRift.Controllers.Buildings.Achievments;
using UniverseRift.Controllers.Common;
using UniverseRift.Controllers.Players.Heroes;
using UniverseRift.GameModelDatas.Cities;
using UniverseRift.Models.Achievments;
using UniverseRift.Models.Results;
using UniverseRift.Models.Voyages;
using UniverseRift.Services.Rewarders;

namespace UniverseRift.Controllers.Buildings.Voyages
{
    public class VoyageController : ControllerBase, IVoyageController
    {
        private const string VOYAGE_NAME = "Voyage";
        private const int MISSION_COUNT = 15;

        private readonly AplicationContext _context;
        private readonly ICommonDictionaries _commonDictionaries;
        private readonly IRewardService _clientRewardService;
        private readonly IAchievmentController _achievmentController;

        public VoyageController(
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

        public async Task NextDay()
        {
            var voyageServerDatas = await _context.VoyageServerDatas.ToListAsync();

            VoyageServerData serverData = null;

            if (voyageServerDatas.Count > 0)
                serverData = voyageServerDatas[0];

            if (serverData == null)
            {
                serverData = new VoyageServerData();
                await _context.VoyageServerDatas.AddAsync(serverData);
                await _context.SaveChangesAsync();
                return;
            }

            serverData.IsDayRest = !serverData.IsDayRest;
            if (!serverData.IsDayRest)
            {
                //var voyageContainer = _commonDictionaries.StorageChallenges[VOYAGE_NAME];
                //for (var i = 0; i < voyageContainer.Missions.Count && i < MISSION_COUNT; i++)
                //{
                    //serverData.Missions.Add(voyageContainer.Missions[i]);
                //}
            }
            else
            {
                var allVoyages = await _context.VoyageDatas.ToListAsync();
                foreach (var voyage in allVoyages)
                    voyage.CurrentMissionIndex = 0;

                //serverData.Missions.Clear();
            }

            await _context.SaveChangesAsync();
        }

        public async Task<VoyageBuildingData> GetPlayerSave(int playerId)
        {
            var allVoyages = await _context.VoyageDatas.ToListAsync();
            var playerVoyage = allVoyages.Find(voyage => voyage.PlayerId == playerId);

            if (playerVoyage == null)
            {
                playerVoyage = new VoyageData()
                {
                    PlayerId = playerId
                };

                await _context.VoyageDatas.AddAsync(playerVoyage);
                await _context.SaveChangesAsync();
            }

            var result = new VoyageBuildingData();
            var voyageServerData = await _context.VoyageServerDatas.FindAsync(1);

            if (voyageServerData == null || voyageServerData.IsDayRest)
                return result;

            var voyageContainer = _commonDictionaries.StorageChallenges[VOYAGE_NAME];
            for (var i = 0; i < voyageContainer.Missions.Count && i < MISSION_COUNT; i++)
            {
                result.Missions.Add(voyageContainer.Missions[i]);
            }

            result.CurrentMissionIndex = playerVoyage.CurrentMissionIndex;
            return result;
        }

        [HttpPost]
        [Route("Voyage/FinishMission")]
        public async Task<AnswerModel> FinishMission(int playerId)
        {
            var answer = new AnswerModel();

            var allVoyages = await _context.VoyageDatas.ToListAsync();
            var playerVoyage = allVoyages.Find(voyage => voyage.PlayerId == playerId);

            if (playerVoyage == null)
            {
                answer.Error = "Not found voyage for this player.";
                return answer;
            }

            var voyageServerData = await _context.VoyageServerDatas.FindAsync(1);

            if (voyageServerData == null || voyageServerData.IsDayRest)
            {
                answer.Error = "Voyage is rest.";
                return answer;
            }

            var voyageContainer = _commonDictionaries.StorageChallenges[VOYAGE_NAME];

            await _clientRewardService
                .AddReward(playerId, voyageContainer.Missions.ToList()[playerVoyage.CurrentMissionIndex].WinReward);

            playerVoyage.CurrentMissionIndex += 1;
            if (playerVoyage.CurrentMissionIndex == MISSION_COUNT)
            {
                await _achievmentController.AchievmentUpdataData(playerId, "CompleteTravelAchievment", 1);
            }
            await _context.SaveChangesAsync();

            answer.Result = "Success";
            return answer;
        }


    }
}
