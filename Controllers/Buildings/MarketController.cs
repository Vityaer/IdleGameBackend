using Cysharp.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniRx;
using UniverseRift.Contexts;
using UniverseRift.Controllers.Players.Inventories.Resources;
using UniverseRift.Controllers.Server;
using UniverseRift.Models.Markets;
using UniverseRift.Models.Resources;
using UniverseRift.Models.Results;

namespace UniverseRift.Controllers.Buildings
{
    public class MarketController : ControllerBase, IDisposable
    {
        private readonly AplicationContext _context;
        private readonly IResourceController _resourceController;
        private readonly ServerController _serverController;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public MarketController(AplicationContext context, IResourceController resourceController, ServerController serverController)
        {
            _context = context;
            _resourceController = resourceController;
            _serverController = serverController;
            _serverController.OnChangeDay.Subscribe(_ => RefreshProducts(RecoveryType.Day).Forget()).AddTo(_disposables);
            _serverController.OnChangeWeek.Subscribe(_ => RefreshProducts(RecoveryType.Week).Forget()).AddTo(_disposables);
            _serverController.OnChangeMonth.Subscribe(_ => RefreshProducts(RecoveryType.Month).Forget()).AddTo(_disposables);
            _serverController.OnChangeGameCycle.Subscribe(_ => RefreshProducts(RecoveryType.GameCycle).Forget()).AddTo(_disposables);
        }


        [HttpPost]
        [Route("Market/BuyProduct")]
        public async Task<AnswerModel> BuyProduct(int playerId, string productName, int count = 1)
        {
            var answer = new AnswerModel();

            if (count <= 0)
            {
                answer.Error = "Count <= 0";
                return answer;
            }

            var products = await _context.Products.ToListAsync();

            var product = products.Find(product => product.Id == productName);
            if (product == null)
            {
                answer.Error = "Product not found";
                return answer;
            }

            var cost = new Resource() { PlayerId = playerId, Type = product.CostType, Count = product.Cost, E10 = product.E10 } * count;

            var permission = await _resourceController.CheckResource(playerId, cost, answer);
            if (!permission)
            {
                return answer;
            }

            var purchases = await _context.Purchases.ToListAsync();
            var purchase = purchases.Find(purchase => purchase.ProductId == product.Id);

            if (purchase != null)
            {
                if (purchase.PurchaseCount + count > product.PurchaseCount)
                {
                    answer.Error = "Already all buy";
                    return answer;
                }

                purchase.PurchaseCount += count;
            }
            else
            {
                if (count > product.PurchaseCount)
                {
                    answer.Error = "Count bigger then can buy";
                    return answer;
                }

                purchase = new Purchase() { PlayerId = playerId, ProductId = product.Id, PurchaseCount = count };
                await _context.Purchases.AddAsync(purchase);
            }

            await _resourceController.SubstactResources(cost);
            await _context.SaveChangesAsync();

            answer.Result = "Success";
            return answer;
        }

        private async UniTaskVoid RefreshProducts(RecoveryType recoveryType)
        {
            var products = await _context.Products.ToListAsync();
            var recoveryProducts = products.FindAll(product => product.RecoveryType == recoveryType);

            var purchases = await _context.Purchases.ToListAsync();

            foreach (var product in recoveryProducts)
            {
                var recoveryPurchases = purchases.FindAll(purchase => purchase.ProductId == product.Id);
                _context.Purchases.RemoveRange(recoveryPurchases);
            }

            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
