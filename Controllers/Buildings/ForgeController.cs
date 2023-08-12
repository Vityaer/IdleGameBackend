using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniverseRift.Contexts;
using UniverseRift.Controllers.Common;
using UniverseRift.Models.Items;
using UniverseRift.Models.Results;

namespace UniverseRift.Controllers.Buildings
{
    public class ForgeController : ControllerBase
    {
        private readonly AplicationContext _context;
        private readonly ICommonDictionaries _commonDictionaries;

        public ForgeController(
            AplicationContext context,
            ICommonDictionaries commonDictionaries
            )
        {
            _commonDictionaries = commonDictionaries;
            _context = context;
        }

        [HttpPost]
        [Route("Forge/CreateItem")]
        public async Task<AnswerModel> CreateItem(int playerId, string itemId, int count = 1)
        {
            var answer = new AnswerModel();

            if (count <= 0)
            {
                answer.Error = $"Data {count} error";
                return answer;
            }

            if (!_commonDictionaries.Items.TryGetValue(itemId, out var itemTemplate))
            {
                answer.Error = $"item id: {itemId} not found";
                return answer;
            }

            if (!_commonDictionaries.ItemRelations.TryGetValue(itemId, out var relation))
            {
                answer.Error = $"Item relation {relation} not found";
                return answer;
            }

            var items = await _context.Items.ToListAsync();
            var playerIngredientItem = items.Find(item => (item.PlayerId == playerId) && (item.Name == relation.ItemIngredientName));
            if (playerIngredientItem == null)
            {
                answer.Error = $"Item ingredients {playerIngredientItem} not found";
                return answer;
            }

            var requireCountItem = relation.RequireCount * count;
            if (playerIngredientItem.Count < requireCountItem)
            {
                answer.Error = $"Item {requireCountItem} not enough";
                return answer;
            }

            var resultItem = items.Find(item => (item.PlayerId == playerId) && (item.Name == relation.ResultItemName));
            playerIngredientItem.Count -= requireCountItem;

            if (resultItem != null)
            {
                resultItem.Count += count;
            }
            else
            {
                resultItem = new Item(playerId, relation.ResultItemName, count);
                await _context.Items.AddAsync(resultItem);
            }

            await _context.SaveChangesAsync();

            answer.Result = "Success";
            return answer;
        }
    }
}
