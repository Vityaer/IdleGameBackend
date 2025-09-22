using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.City.Mines;
using UniverseRift.Contexts;
using UniverseRift.Controllers.Buildings.Industries.Mines;
using UniverseRift.Controllers.Common;
using UniverseRift.GameModelDatas.Cities.Industries;
using UniverseRift.Heplers.Utils;

namespace UniverseRift.Controllers.Buildings.Industries
{
	public class IndustryController : ControllerBase, IIndustryController
	{
		private const string MAIN_MINE_NAME = "MainMineBuilding";

		private readonly ICommonDictionaries _commonDictionaries;
		private readonly IMineController _mineController;
		private readonly AplicationContext _context;
		private readonly Random _random;

		public IndustryController(
			AplicationContext context,
			ICommonDictionaries commonDictionaries,
			IMineController mineController
			)
		{
			_context = context;
			_commonDictionaries = commonDictionaries;
			_mineController = mineController;

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

			var industryData = await _context.IndustryServerDatas.SingleOrDefaultAsync(data => data.PlayerId == playerId);

			if (industryData != null)
			{
				var timeCreate = DateTimeUtils.TryParseOrNow(industryData.DateTimeCreate);
				var dateTimeRefresh = timeCreate.AddHours(Constants.Game.MINE_MISSION_REFRESH_HOURS);

				if (dateTimeRefresh <= DateTime.UtcNow)
				{
					await _mineController.RefreshMissions(playerId);
				}

				result.MineEnergy = industryData.MineEnergy;
				result.DateTimeCreate = industryData.DateTimeCreate;
			}

			var allPlayerMissions = await _context.MineMissionDatas
				.Where(p => p.PlayerId == playerId)
				.ToListAsync();

			result.MissionDatas = allPlayerMissions
				.Where(p => p.PlayerId == playerId && p.StorageMissionContainerId.Contains("MineTravelLevel_"))
				.ToList();

			result.BossMissionData = allPlayerMissions.Find(data => data.StorageMissionContainerId.Contains("MineBossTravelLevel_"));

			return result;
		}

		public async Task OnPlayerRegister(int playerId)
		{
			var playerIndustry = new IndustryServerData();
			playerIndustry.PlayerId = playerId;

			var mineBuildingModel = _commonDictionaries.Buildings[MAIN_MINE_NAME] as MineBuildingModel;
			if (mineBuildingModel != null)
			{
				if (mineBuildingModel.ConfigureContainers.Count > 0)
				{
					playerIndustry.MineEnergy = mineBuildingModel.ConfigureContainers[0].MaxEnergyCount;
				}
			}

			await _context.IndustryServerDatas.AddAsync(playerIndustry);
			await _context.SaveChangesAsync();
		}
	}
}
