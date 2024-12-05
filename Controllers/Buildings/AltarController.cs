using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Misc.Json;
using Models.Data.Inventories;
using Newtonsoft.Json;
using UniverseRift.Contexts;
using UniverseRift.GameModels;
using UniverseRift.GameModels.Common;
using UniverseRift.Models.Heroes;
using UniverseRift.Models.Resources;
using UniverseRift.Models.Results;

namespace UniverseRift.Controllers.Buildings
{
    public class AltarController : ControllerBase
    {
        private readonly AplicationContext _context;
        private readonly IJsonConverter _jsonConverter;

        public AltarController(
            AplicationContext context,
            IJsonConverter jsonConverter
            )
        {
            _jsonConverter = jsonConverter;
            _context = context;
        }

        [HttpPost]
        [Route("Altar/RemoveHeroes")]
        public async Task<AnswerModel> RemoveHeroes(int playerId, string jsonContainer)
        {
            var answer = new AnswerModel();
            var fireContainer = JsonConvert.DeserializeObject<FireContainer>(jsonContainer);

            if (fireContainer == null)
            {
                answer.Error = "Wrong data";
                return answer;
            }

            var heroIds = fireContainer.HeroesIds;
            var heroCount = heroIds.Count;

            if (heroIds.Count == 0)
            {
                answer.Error = $"data error, hero count: {heroIds.Count}";
                return answer;
            }

            var heroes = await _context.Heroes.ToListAsync();
            foreach (var id in heroIds)
            {
                var hero = heroes.Find(hero => (hero.PlayerId == playerId) && (hero.Id == id));
                if (hero != null)
                    _context.Heroes.Remove(hero);
            }

            var reward = new RewardModel();
            var resource = new ResourceData { Type = ResourceType.RedDust, Amount = new BigDigit(10 * heroCount) };
            reward.Add(resource);

            await _context.SaveChangesAsync();

            answer.Result = _jsonConverter.Serialize(reward);
            return answer;
        }
    }
}
