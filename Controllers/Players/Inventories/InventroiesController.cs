using Microsoft.AspNetCore.Mvc;
using UniverseRift.Contexts;
using UniverseRift.Models.Common;

namespace UniverseRift.Controllers.Players.Inventories
{
    public class InventroiesController : Controller
    {
        private readonly AplicationContext _context;

        public InventroiesController(AplicationContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("Heroes/GetInventory")]
        public async Task<Inventory> GetInventory(int playerId)
        {
            var inventory = new Inventory
            (
                _context.Resources.ToList().FindAll(res => res.PlayerId == playerId),
                _context.Items.ToList().FindAll(item => item.PlayerId == playerId),
                _context.Splinters.ToList().FindAll(splinter => splinter.PlayerId == playerId)
            );

            return inventory;
        }

        /*[HttpPost]
        [Route("Heroes/GiveReward")]
        public async Task<Inventory> GiveReward(int playerId, string rewardJSON)
        {
            var reward = 
            var inventory = new Inventory
            (
                _context.Resources.ToList().FindAll(res => res.PlayerId == playerId),
                _context.Items.ToList().FindAll(item => item.PlayerId == playerId),
                _context.Splinters.ToList().FindAll(splinter => splinter.PlayerId == playerId)
            );

            return inventory;
        }*/
    }
}
