using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniverseRift.Contexts;
using UniverseRift.Models.Heroes;
using UniverseRift.Models.Results;
using Misc.Json;
using UniverseRift.MessageData;

namespace UniverseRift.Controllers.Buildings
{
    public class SanctuaryController : ControllerBase
    {
        private readonly AplicationContext _context;
        private readonly IJsonConverter _jsonConverter;
        private readonly Random _random = new Random();

        public SanctuaryController(AplicationContext context, IJsonConverter jsonConverter)
        {
            _context = context;
            _jsonConverter = jsonConverter;
        }

        [HttpPost]
        [Route("Sanctuary/ReplaceHero")]
        public async Task<AnswerModel> ReplaceHero(int playerId, int heroId)
        {
            var answer = new AnswerModel();
            var hero = await _context.Heroes.FindAsync(heroId);

            if (hero == null)
            {
                answer.Error = "Hero not found";
                return answer;
            }

            if (hero.PlayerId != playerId)
            {
                answer.Error = "Hero belongs other player";
                return answer;
            }

            var heroTemplates = await _context.HeroTemplates.ToListAsync();
            var oldHeroTemplate = heroTemplates.Find(template => template.Id == hero.HeroTemplateId);
            
            if (oldHeroTemplate == null)
            {
                answer.Error = "Hero template not found";
                return answer;
            }

            var raceHeros = heroTemplates.FindAll(template => template.Race == oldHeroTemplate.Race);
            raceHeros.Remove(oldHeroTemplate);

            var randomIndex = _random.Next(0, raceHeros.Count);
            var newHero = new Hero(playerId, raceHeros[randomIndex]);

            _context.Heroes.Remove(hero);
            await _context.Heroes.AddAsync(newHero);
            await _context.SaveChangesAsync();

            var heroData = new HeroData(newHero);
            answer.Result = _jsonConverter.Serialize(heroData);
            return answer;
        }
    }
}
