using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniverseRift.Contexts;
using UniverseRift.Controllers.Common;
using UniverseRift.Controllers.Players.Heroes;
using UniverseRift.Models.Items;
using UniverseRift.Models.Results;

namespace UniverseRift.Controllers.Players.Inventories.Items
{
    public class ItemsController : Controller, IItemsController
    {
        private readonly AplicationContext _context;
        private readonly IHeroesController _heroesController;
        private readonly ICommonDictionaries _commonDictionaries;

        public ItemsController(
            AplicationContext context,
            IHeroesController heroesController,
            ICommonDictionaries commonDictionaries
            )
        {
            _commonDictionaries = commonDictionaries;
            _context = context;
            _heroesController = heroesController;
        }

        [HttpPost]
        [Route("Items/SetItem")]
        public async Task<AnswerModel> SetItemOnHero(int playerId, int heroId, string ItemId)
        {
            var answer = new AnswerModel();
            var hero = await _heroesController.GetHero(playerId, heroId);

            if (hero == null)
            {
                answer.Error = $"Hero id error: {heroId}";
                return answer;
            }

            if (!_commonDictionaries.Items.ContainsKey(ItemId))
            {
                answer.Error = $"Error data: {ItemId}";
                return answer;
            }

            var itemTemplate = _commonDictionaries.Items[ItemId];

            await TakeOffItemFromHero(playerId, heroId, (int) itemTemplate.Type);

            await RemoveItem(playerId, ItemId);
            switch (itemTemplate.Type)
            {
                case ItemType.Weapon:
                    hero.WeaponItemId = itemTemplate.Id;
                    break;
                case ItemType.Armor:
                    hero.ArmorItemId = itemTemplate.Id;
                    break;
                case ItemType.Boots:
                    hero.BootsItemId = itemTemplate.Id;
                    break;
                case ItemType.Amulet:
                    hero.AmuletItemId = itemTemplate.Id;
                    break;
            }
            await _context.SaveChangesAsync();
            answer.Result = Constants.Common.SUCCESS_RUSULT;
            return answer;
        }

        [HttpPost]
        [Route("Items/TakeOffItem")]
        public async Task<AnswerModel> TakeOffItemFromHero(int playerId, int heroId, int intItemType)
        {
            var answer = new AnswerModel();

            var hero = await _heroesController.GetHero(playerId, heroId);

            if (hero == null)
            {
                answer.Error = $"Data error: {heroId}";
                return answer;
            }

            var itemType = (ItemType)intItemType;

            var currentItemName = string.Empty;
            switch (itemType)
            {
                case ItemType.Weapon:
                    currentItemName = hero.WeaponItemId;
                    hero.WeaponItemId = string.Empty;
                    break;
                case ItemType.Armor:
                    currentItemName = hero.ArmorItemId;
                    hero.ArmorItemId = string.Empty;
                    break;
                case ItemType.Boots:
                    currentItemName = hero.BootsItemId;
                    hero.BootsItemId = string.Empty;
                    break;
                case ItemType.Amulet:
                    currentItemName = hero.AmuletItemId;
                    hero.AmuletItemId = string.Empty;
                    break;
            }

            if (!string.IsNullOrEmpty(currentItemName))
            {
                await AddItem(playerId, currentItemName);
            }
            await _context.SaveChangesAsync();
            answer.Result = "Success";
            return answer;
        }

        public async Task AddItem(int playerId, string itemName, int count = 1)
        {
            if (!_commonDictionaries.Items.ContainsKey(itemName))
                return;

            var items = await _context.Items.ToListAsync();
            var item = items.Find(item => item.PlayerId == playerId && item.Name == itemName);

            if (item != null)
            {
                item.Count += count;
            }
            else
            {
                var newItem = new Item(playerId, itemName, count);
                _context.Items.Add(newItem);
            }
            await _context.SaveChangesAsync();
        }

        public async Task RemoveItem(int playerId, string itemName, int count = 1)
        {
            var items = await _context.Items.ToListAsync();
            var item = items.Find(item => item.PlayerId == playerId && item.Name == itemName);

            if (item == null)
                return;

            item.Count -= count;
            if (item.Count == 0)
            {
                _context.Items.Remove(item);
            }

            await _context.SaveChangesAsync();
        }
    }
}
