using Cysharp.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using UniRx;
using UniverseRift.Contexts;
using UniverseRift.Controllers.Common;
using UniverseRift.Controllers.Server;
using UniverseRift.Models.City.Markets;
using UniverseRift.Models.Resources;
using UniverseRift.Models.Results;
using UniverseRift.Services.Resources;
using UniverseRift.Services.Rewarders;

namespace UniverseRift.Controllers.Buildings.Shops
{
    public class MarketController : ControllerBase, IMarketController, IDisposable
    {
        private readonly AplicationContext _context;
        private readonly IResourceManager _resourceController;
        private readonly ICommonDictionaries _commonDictionaries;
        private readonly IRewardService _clientRewardService;
        private readonly IServerController _serverController;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public MarketController(
            AplicationContext context,
            IRewardService clientRewardService,
            IResourceManager resourceController,
            IServerController serverController,
            ICommonDictionaries commonDictionaries)
        {
            _clientRewardService = clientRewardService;
            _context = context;
            _resourceController = resourceController;
            _serverController = serverController;
            _commonDictionaries = commonDictionaries;
            _serverController.OnChangeDay.Subscribe(_ => RefreshProducts(RecoveryType.Day).Forget()).AddTo(_disposables);
            _serverController.OnChangeWeek.Subscribe(_ => RefreshProducts(RecoveryType.Week).Forget()).AddTo(_disposables);
            _serverController.OnChangeMonth.Subscribe(_ => RefreshProducts(RecoveryType.Month).Forget()).AddTo(_disposables);
            _serverController.OnChangeGameCycle.Subscribe(_ => RefreshProducts(RecoveryType.GameCycle).Forget()).AddTo(_disposables);
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

        private async UniTaskVoid RefreshProducts(RecoveryType recoveryType)
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
                    _context.Purchases.RemoveRange(recoveryPurchases);
                }
            }

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
            return result;
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
