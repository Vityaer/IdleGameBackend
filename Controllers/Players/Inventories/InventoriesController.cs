using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Data.Inventories;
using UniverseRift.Contexts;
using UniverseRift.GameModelDatas.Players;
using UniverseRift.GameModels;
using UniverseRift.Models.Items;

namespace UniverseRift.Controllers.Players.Inventories
{
    public class InventoriesController : Controller, IInventoriesController
    {
        private readonly AplicationContext _context;

        public InventoriesController(AplicationContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("Heroes/GetInventory")]
        public async Task<InventoryData> GetInventory(int playerId)
        {
            var result = new InventoryData();
            var items = await _context.Items.ToListAsync();
            var playerItems = items.FindAll(item => item.PlayerId == playerId);

            foreach (var item in playerItems)
            {
                result.Items.Add(new ItemData { Id = item.Name, Amount = (int)Math.Round(item.Count) });
            }

            var splinters = await _context.Splinters.ToListAsync();
            var playerSplinters = splinters.FindAll(splinterData => splinterData.PlayerId == playerId);

            foreach (var splinter in playerSplinters)
            {
                result.Splinters.Add(new SplinterData { Id = splinter.SplinterId, Amount = (int)Math.Round(splinter.Count) } );
            }

            return result;
        }
    }
}
