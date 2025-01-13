using Cysharp.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using UniRx;
using UniverseRift.Contexts;
using UniverseRift.Controllers.Buildings.Achievments;
using UniverseRift.Controllers.Common;
using UniverseRift.Heplers.Utils;
using UniverseRift.Models.City.Markets;
using UniverseRift.Models.Resources;
using UniverseRift.Models.Results;
using UniverseRift.Services.Resources;
using UniverseRift.Services.Rewarders;

namespace UniverseRift.Controllers.Buildings.Shops
{
    public class MarketController : ControllerBase, IMarketController, IDisposable
    {
        private const string CITY_MARKET_NAME = "CityMarket";
        private const int CITY_MARKET_PROMO_PRODUCT_COUNT = 4;
        private TimeSpan ALCHEMY_TIMESPAN = new TimeSpan(8, 0, 0);

        private readonly AplicationContext _context;
        private readonly IResourceManager _resourceController;
        private readonly ICommonDictionaries _commonDictionaries;
        private readonly IRewardService _clientRewardService;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private readonly IAchievmentController _achievmentController;

        private readonly Random _random = new();

        public MarketController(
            AplicationContext context,
            IRewardService clientRewardService,
            IResourceManager resourceController,
            ICommonDictionaries commonDictionaries,
            IAchievmentController achievmentController)
        {
            _clientRewardService = clientRewardService;
            _context = context;
            _resourceController = resourceController;
            _commonDictionaries = commonDictionaries;
            _achievmentController = achievmentController;
        }

        public async Task OnStartServer()
        {
            await CreateDayPromotions();
        }

        [HttpPost]
        [Route("Market/GetAlchemy")]
        public async Task<AnswerModel> GetAlchemy(int playerId)
        {
            var answer = new AnswerModel();
            var player = await _context.Players.FindAsync(playerId);

            if (player == null)
            {
                answer.Error = "Not found player";
                return answer;
            }
            var lastGetAlchemyGameData = DateTimeUtils.TryParseOrNow(player.LastGetAlchemyDateTime);
            var delta = DateTime.UtcNow - lastGetAlchemyGameData;

            if (delta < ALCHEMY_TIMESPAN)
            {
                answer.Error = "Early, wait time";
                return answer;
            }

            await _achievmentController.AchievmentUpdataData(playerId, "GetAlchemyGoldCountAchievment", 1);

            var rewardData = _commonDictionaries.Rewards["Alchemy"];
            await _clientRewardService.AddReward(player.Id, rewardData);

            player.LastGetAlchemyDateTime = DateTime.UtcNow.ToString(Constants.Common.DateTimeFormat);
            await _context.SaveChangesAsync();
            answer.Result = "Success";
            return answer;
        }

        [HttpPost]
        [Route("Market/BuyProduct")]
        public async Task<AnswerModel> BuyProduct(int playerId, string productId, int count = 1)
        {
            var answer = new AnswerModel();

            if (count <= 0)
            {
                answer.Error = "Count <= 0";
                return answer;
            }

            if (!_commonDictionaries.Products.ContainsKey(productId))
            {
                answer.Error = "Product not found";
                return answer;
            }

            var product = _commonDictionaries.Products[productId];

            var cost = new Resource() { PlayerId = playerId, Type = product.Cost.Type, Count = product.Cost.Amount.Mantissa, E10 = product.Cost.Amount.E10 } * count;

            var permission = await _resourceController.CheckResource(playerId, cost, answer);
            if (!permission)
            {
                return answer;
            }

            var purchases = await _context.Purchases.ToListAsync();
            var playerPurchases = purchases.FindAll(purchase => purchase.PlayerId == playerId);

            var purchase = playerPurchases.Find(purchase => purchase.ProductId == productId);
            if (purchase != null)
            {
                if (purchase.PurchaseCount + count > product.CountSell)
                {
                    answer.Error = "Already all buy";
                    return answer;
                }

                purchase.PurchaseCount += count;
            }
            else
            {
                if (count > product.CountSell)
                {
                    answer.Error = "Count bigger then can buy";
                    return answer;
                }

                purchase = new Purchase() { PlayerId = playerId, ProductId = product.Id, PurchaseCount = count };
                await _context.Purchases.AddAsync(purchase);
            }

            await _resourceController.SubstactResources(cost);
            await _clientRewardService.AddProduct(playerId, product);

            await _context.SaveChangesAsync();

            answer.Result = "Success";
            return answer;
        }

        public async Task RefreshProducts(RecoveryType recoveryType)
        {
            var products = _commonDictionaries.Products;
            var markets = _commonDictionaries.Markets;

            var recoveryMarkets = markets.Values.ToList().FindAll(market => market.RecoveryType == recoveryType);
            var purchases = await _context.Purchases.ToListAsync();

            foreach (var market in recoveryMarkets)
            {
                var recoveryProducts = products
                                        .Where(product => market.Products.Contains(product.Value.Id))
                                        .Select(product => product.Value)
                                        .ToList();


                foreach (var product in recoveryProducts)
                {
                    var recoveryPurchases = purchases.FindAll(purchase => purchase.ProductId == product.Id);
                    if (recoveryPurchases.Count > 0)
                    {
                        _context.Purchases.RemoveRange(recoveryPurchases);
                    }
                }
            }

            switch (recoveryType)
            {
                case RecoveryType.Day:
                    await CreateDayPromotions();
                        break;
            }

            await _context.SaveChangesAsync();
        }

        private async Task CreateDayPromotions()
        {
            var allPromotions = await _context.Promotions.ToListAsync();
            var oldCityMarketPromotions = allPromotions.FindAll(promo => promo.MarketName.Equals(CITY_MARKET_NAME));
            _context.Promotions.RemoveRange(oldCityMarketPromotions);
            await _context.SaveChangesAsync();

            var cityMarketProductIds = _commonDictionaries.Products.Keys.ToList()
                .FindAll(name => name.Contains($"Promo{CITY_MARKET_NAME}"));

            var indexes = new List<int>(cityMarketProductIds.Count);
            for (var i = 0; i < cityMarketProductIds.Count; i++)
                indexes.Add(i);

            var selectedIndexes = new List<int>(CITY_MARKET_PROMO_PRODUCT_COUNT);
            for (var i = 0; i < CITY_MARKET_PROMO_PRODUCT_COUNT; i++)
            {
                var randomIndex = _random.Next(0, indexes.Count);
                selectedIndexes.Add(indexes[randomIndex]);
                indexes.RemoveAt(randomIndex);
            }

            var promotions = new List<Promotion>(selectedIndexes.Count);
            for (var i = 0; i < selectedIndexes.Count; i++)
            {
                var productId = cityMarketProductIds[selectedIndexes[i]];
                promotions.Add(new Promotion {MarketName = CITY_MARKET_NAME, ProductId = productId });
            }

            await _context.Promotions.AddRangeAsync(promotions);
            await _context.SaveChangesAsync();
        }

        public async Task<MarketData> GetPlayerSave(int playerId)
        {
            var result = new MarketData();
            var purchases = await _context.Purchases.ToListAsync();
            var playerPurchases = purchases.FindAll(purchase => purchase.PlayerId == playerId);

            result.PurchaseDatas = playerPurchases
                                    .Select(purchase =>
                                        new PurchaseData
                                        {
                                            ProductId = purchase.ProductId,
                                            PurchaseCount = purchase.PurchaseCount
                                        }
                                     ).ToList();

            result.ShopPromotions = await _context.Promotions.ToListAsync();
            return result;
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

    }
}
