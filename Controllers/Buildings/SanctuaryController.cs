using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniverseRift.Contexts;
using UniverseRift.Models.Heroes;
using UniverseRift.Models.Results;
using Misc.Json;
using UniverseRift.MessageData;
using Microsoft.Extensions.Hosting;
using UniverseRift.GameModels.Heroes;
using UniverseRift.Controllers.Common;

namespace UniverseRift.Controllers.Buildings
{
    public class SanctuaryController : ControllerBase
    {
        private readonly AplicationContext _context;
        private readonly IJsonConverter _jsonConverter;
        private readonly Random _random = new Random();
        private readonly ICommonDictionaries _commonDictionaries;

        public SanctuaryController(
            AplicationContext context,
            IJsonConverter jsonConverter,
            ICommonDictionaries commonDictionaries)
        {
            _context = context;
            _jsonConverter = jsonConverter;
            _commonDictionaries = commonDictionaries;
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

            var allHeroes = _commonDictionaries.Heroes;
            var workList = new List<HeroModel>();
            HeroModel heroTemplate;

            var heroTemplates = _commonDictionaries.Heroes;
            var oldHeroTemplate = heroTemplates[hero.HeroTemplateId];
            
            if (oldHeroTemplate == null)
            {
                answer.Error = "Hero template not found";
                return answer;
            }

            var raceHeroes = heroTemplates
                .Where(template => template.Value.General.Race == oldHeroTemplate.General.Race)
                .Select(x => x.Value)
                .ToList();

            raceHeroes.Remove(oldHeroTemplate);

            var randomIndex = _random.Next(0, raceHeroes.Count);
            var newHero = new Hero(playerId, raceHeroes[randomIndex]);

            _context.Heroes.Remove(hero);
            await _context.Heroes.AddAsync(newHero);
            await _context.SaveChangesAsync();

            var heroData = new HeroData(newHero);
            answer.Result = _jsonConverter.Serialize(heroData);
            return answer;
        }
    }
}
