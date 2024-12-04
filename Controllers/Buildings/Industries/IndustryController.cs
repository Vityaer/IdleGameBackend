using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniverseRift.Contexts;
using UniverseRift.Controllers.Common;
using UniverseRift.GameModelDatas.Cities.Industries;
using UniverseRift.Heplers.Utils;

namespace UniverseRift.Controllers.Buildings.Industries
{
    public class IndustryController : ControllerBase, IIndustryController
    {
        private const string MAIN_MINE_NAME = "MainMineBuilding";

        private readonly ICommonDictionaries _commonDictionaries;

        private readonly AplicationContext _context;
        private readonly Random _random;

        public IndustryController(
            AplicationContext context,
            ICommonDictionaries commonDictionaries
            )
        {
            _context = context;
            _commonDictionaries = commonDictionaries;

            _random = new Random();
        }

        public async Task<IndustryData> GetPlayerSave(int playerId, bool flagCreateNewData)
        {
            var result = new IndustryData();
            var mineDatas = await _context.MineDatas.ToListAsync();
            var playerMineDatas = mineDatas.FindAll(data => data.PlayerId == playerId);
            result.Mines.AddRange(playerMineDatas);

            var mineMissions = await _context.MineMissionDatas.ToListAsync();
            var playerMineMissionDatas = mineMissions.FindAll(data => data.PlayerId == playerId);

            var MineMissionsForRefresh = new List<MineMissionData>();
            foreach (var mission in playerMineMissionDatas)
            {
                var timeCreate = DateTimeUtils.TryParseOrNow(mission.DateTimeCreate);
                var dateTimeRefresh = timeCreate.AddHours(Constants.Game.MINE_MISSION_REFRESH_HOURS);

                if (dateTimeRefresh <= DateTime.UtcNow)
                {
                    MineMissionsForRefresh.Add(mission);
                }
            }

            if (MineMissionsForRefresh.Count > 0)
            {
                var mainBuilding = mineDatas.Find(mine => mine.MineId.Equals(MAIN_MINE_NAME));
                if (mainBuilding != null)
                {
                    await RefreshMissions(mainBuilding, MineMissionsForRefresh);
                }
            }

            result.MissionDatas.AddRange(playerMineMissionDatas);

            return result;
        }

        private async Task RefreshMissions(MineData mainMineBuilding, List<MineMissionData> mineMissionsForRefresh)
        {
            var travelLevel = mainMineBuilding.Level / 3;
            var tavelName = $"MineTravelLevel_{travelLevel}";
            var travel = _commonDictionaries.StorageChallenges[tavelName];

            foreach (var mission in mineMissionsForRefresh)
            {
                var randIndex = _random.Next(travel.Missions.Count);
                var travelMission = travel.Missions[randIndex];

                mission.MissionId = travelMission.Name;
                mission.DateTimeCreate = DateTime.UtcNow.ToString(Constants.Common.DateTimeFormat);
                mission.IsComplete = false;
            }

            await _context.SaveChangesAsync();
        }
    }
}
