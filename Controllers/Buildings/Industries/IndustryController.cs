using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.City.Mines;
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

			var industryDatas = await _context.IndustryServerDatas.ToListAsync();
			var playerIndustry = industryDatas.Find(data => data.PlayerId == playerId);

			if (playerIndustry != null)
			{
				result.MineEnergy = playerIndustry.MineEnergy;
			}

			return result;
		}

		public async Task OnPlayerRegister(int playerId)
		{
			var playerIndustry = new IndustryServerData();
			playerIndustry.PlayerId = playerId;

			var mineBuildingModel = _commonDictionaries.Buildings[MAIN_MINE_NAME] as MineBuildingModel;
			if (mineBuildingModel != null)
			{
				if (mineBuildingModel.MineEnergyDatas.Count > 0)
				{
					playerIndustry.MineEnergy = mineBuildingModel.MineEnergyDatas[0].MaxEnergyCount;
				}
			}

			await _context.IndustryServerDatas.AddAsync(playerIndustry);
			await _context.SaveChangesAsync();
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
