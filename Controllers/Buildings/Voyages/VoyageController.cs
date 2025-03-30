using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using UniverseRift.Contexts;
using UniverseRift.Controllers.Buildings.Achievments;
using UniverseRift.Controllers.Common;
using UniverseRift.Controllers.Players.Heroes;
using UniverseRift.GameModelDatas.Cities;
using UniverseRift.GameModels;
using UniverseRift.Models.Achievments;
using UniverseRift.Models.City.Markets;
using UniverseRift.Models.Resources;
using UniverseRift.Models.Results;
using UniverseRift.Models.Voyages;
using UniverseRift.Services.Rewarders;

namespace UniverseRift.Controllers.Buildings.Voyages
{
    public class VoyageController : ControllerBase, IVoyageController
    {
		private const string MARKET_NAME = "VoyageMarket";
        private const int CITY_MARKET_PROMO_PRODUCT_COUNT = 5;

		private const string VOYAGE_NAME = "Voyage";
        private const int MISSION_COUNT = 15;

		private readonly AplicationContext _context;
        private readonly ICommonDictionaries _commonDictionaries;
        private readonly IRewardService _clientRewardService;
        private readonly IAchievmentController _achievmentController;

		private readonly Random _random = new();

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
            }

            serverData.IsVoyageDay = !serverData.IsVoyageDay;
			if (serverData.IsVoyageDay)
            {
				var allVoyages = await _context.VoyageDatas.ToListAsync();
				foreach (var voyage in allVoyages)
					voyage.CurrentMissionIndex = 0;

				//serverData.Missions.Clear();
				await CreateDayPromotions();
			}
            else
            {
				//var voyageContainer = _commonDictionaries.StorageChallenges[VOYAGE_NAME];
				//for (var i = 0; i < voyageContainer.Missions.Count && i < MISSION_COUNT; i++)
				//{
				//serverData.Missions.Add(voyageContainer.Missions[i]);
				//}
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

            if (voyageServerData == null || !voyageServerData.IsVoyageDay)
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

            if (voyageServerData == null || !voyageServerData.IsVoyageDay)
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

		private async Task CreateDayPromotions()
		{
			var allPromotions = await _context.Promotions.ToListAsync();
			var oldCityMarketPromotions = allPromotions.FindAll(promo => promo.MarketName.Equals(MARKET_NAME));
			_context.Promotions.RemoveRange(oldCityMarketPromotions);
			await _context.SaveChangesAsync();

			var cityMarketProductIds = _commonDictionaries.Products.Keys.ToList()
				.FindAll(name => name.Contains($"Promo{MARKET_NAME}"));

            var createPromoCount = Math.Min(CITY_MARKET_PROMO_PRODUCT_COUNT, cityMarketProductIds.Count);
			var indexes = new List<int>(createPromoCount);
			for (var i = 0; i < createPromoCount; i++)
				indexes.Add(i);

			var selectedIndexes = new List<int>(createPromoCount);
			for (var i = 0; i < createPromoCount; i++)
			{
				var randomIndex = _random.Next(0, indexes.Count);
				selectedIndexes.Add(indexes[randomIndex]);
				indexes.RemoveAt(randomIndex);
			}

			var promotions = new List<Promotion>(selectedIndexes.Count);
			for (var i = 0; i < selectedIndexes.Count; i++)
			{
				var productId = cityMarketProductIds[selectedIndexes[i]];
				promotions.Add(new Promotion { MarketName = MARKET_NAME, ProductId = productId });
			}

			await _context.Promotions.AddRangeAsync(promotions);
			await _context.SaveChangesAsync();
		}
	}
}
