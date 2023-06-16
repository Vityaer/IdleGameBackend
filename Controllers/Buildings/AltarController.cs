using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniverseRift.Contexts;

namespace UniverseRift.Controllers.Buildings
{
    public class AltarController : ControllerBase
    {
        private readonly AplicationContext _context;

        public AltarController(AplicationContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("Altar/RemoveHeroes")]
        public async Task RemoveHeroes(int playerId, List<int> heroIds)
        {
            if (heroIds.Count == 0)
                return;

            var heroes = await _context.Heroes.ToListAsync();
            foreach (var id in heroIds)
            {
                var hero = heroes.Find(hero => (hero.PlayerId == playerId) && (hero.Id == id));
                if(hero != null)
                    _context.Heroes.Remove(hero);
            }

            await _context.SaveChangesAsync();
        }
    }
}
