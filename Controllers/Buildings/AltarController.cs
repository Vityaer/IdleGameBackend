using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniverseRift.Contexts;
using UniverseRift.Models.Results;

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
        public async Task<AnswerModel> RemoveHeroes(int playerId, List<int> heroIds)
        {
            var answer = new AnswerModel();
            if (heroIds.Count == 0)
            {
                answer.Error = $"data error, hero count: {heroIds.Count}";
                return answer;
            }

            var heroes = await _context.Heroes.ToListAsync();
            foreach (var id in heroIds)
            {
                var hero = heroes.Find(hero => (hero.PlayerId == playerId) && (hero.Id == id));
                if(hero != null)
                    _context.Heroes.Remove(hero);
            }

            await _context.SaveChangesAsync();

            answer.Result = "Success";
            return answer;
        }
    }
}
