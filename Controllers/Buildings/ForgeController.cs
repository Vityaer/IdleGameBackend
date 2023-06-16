using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniverseRift.Contexts;
using UniverseRift.Models.Items;
using UniverseRift.Models.Results;

namespace UniverseRift.Controllers.Buildings
{
    public class ForgeController : ControllerBase
    {
        private readonly AplicationContext _context;

        public ForgeController(AplicationContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("Forge/CreateItem")]
        public async Task<AnswerModel> CreateItem(int playerId, string itemTemplateName, int count = 1)
        {
            var answer = new AnswerModel();

            if (count <= 0)
            {
                answer.Error = "Data {count} error";
                return answer;
            }

            var itemTemplates = await _context.ItemTemplates.ToListAsync();
            var itemTemplate = itemTemplates.Find(template => template.Name == itemTemplateName);
            if (itemTemplate == null)
            {
                answer.Error = "Data {itemTemplateName} error";
                return answer;
            }
            
            var itemSynthesisRelations = await _context.ItemSynthesisRelations.ToListAsync();
            var relation = itemSynthesisRelations.Find(relation => relation.ResultItemName == itemTemplateName);
            if (relation == null)
            {
                answer.Error = "Item {Relation} not found";
                return answer;
            }

            var items = await _context.Items.ToListAsync();
            var playerIngredientItem = items.Find(item => (item.PlayerId == playerId) && (item.Name == relation.ItemIngredientName));
            if (playerIngredientItem == null) 
            {
                answer.Error = "Item {playerIngredientItem} not found";
                return answer;
            }

            var requireCountItem = relation.RequireCount * count;
            if (playerIngredientItem.Count < requireCountItem)
            {
                answer.Error = "Item {requireCountItem} not enough";
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
