using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniverseRift.Contexts;
using UniverseRift.Controllers.Players.Heroes;
using UniverseRift.Models.Items;

namespace UniverseRift.Controllers.Players.Inventories.Items
{
    public class ItemsController : Controller
    {
        private readonly AplicationContext _context;
        private readonly IHeroesController _heroesController;

        public ItemsController(AplicationContext context, IHeroesController heroesController)
        {
            _context = context;
            _heroesController = heroesController;
        }

        [HttpPost]
        [Route("Items/SetItem")]
        public async Task SetItemOnHero(int playerId, int heroId, string itemName)
        {
            //TODO: await many tasks
            var items = await _context.Items.ToListAsync();
            var hero = await _heroesController.GetHero(playerId, heroId);

            var itemTemplates = await _context.ItemTemplates.ToListAsync();
            var itemTemplate = itemTemplates.Find(template => template.Name == itemName);

            if (itemTemplate == null)
                return;

            await TakeOffItemFromHero(playerId, heroId, itemTemplate.Type);

            await RemoveItem(playerId, itemName);
            switch (itemTemplate.Type)
            {
                case ItemType.Weapon:
                    hero.WeaponItemId = itemTemplate.Name;
                    break;
                case ItemType.Armor:
                    hero.ArmorItemId = itemTemplate.Name;
                    break;
                case ItemType.Boots:
                    hero.BootsItemId = itemTemplate.Name;
                    break;
                case ItemType.Amulet:
                    hero.AmuletItemId = itemTemplate.Name;
                    break;
            }
        }

        [HttpPost]
        [Route("Items/TakeOffItem")]
        public async Task TakeOffItemFromHero(int playerId, int heroId, ItemType itemType)
        {
            var hero = await _heroesController.GetHero(playerId, heroId);

            if (hero == null)
                return;

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

            if (string.IsNullOrEmpty(currentItemName))
                return;

            await AddItem(playerId, currentItemName);
        }

        [HttpPost]
        [Route("Items/AddItem")]
        public async Task AddItem(int playerId, string itemName)
        {
            var items = await _context.Items.ToListAsync();
            var item = items.Find(item => item.PlayerId == playerId && item.Name == itemName);

            if (item != null)
            {
                item.Count += 1;
            }
            else
            {
                var newItem = new Item(playerId, itemName);
                items.Add(newItem);
            }
            await _context.SaveChangesAsync();
        }

        [HttpPost]
        [Route("Items/RemoveItem")]
        public async Task RemoveItem(int playerId, string itemName)
        {
            var items = await _context.Items.ToListAsync();
            var item = items.Find(item => item.PlayerId == playerId && item.Name == itemName);

            if (item == null)
                return;

            item.Count -= 1;
            await _context.SaveChangesAsync();
        }

        // GET: Items
        public async Task<IActionResult> Index()
        {
            return _context.Items != null ?
                        View(await _context.Items.ToListAsync()) :
                        Problem("Entity set 'AplicationContext.Items'  is null.");
        }

        // GET: Items/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Items == null)
            {
                return NotFound();
            }

            var item = await _context.Items
                .FirstOrDefaultAsync(m => m.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Type,Id,PlayerId,Count,E10")] Item item)
        {
            if (ModelState.IsValid)
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }
    }
}
