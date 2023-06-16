using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniverseRift.Contexts;
using UniverseRift.Controllers.Players.Inventories.Resources;
using UniverseRift.Misc;
using UniverseRift.Models.Heroes;
using UniverseRift.Models.Inventories.Resources;

namespace UniverseRift.Controllers.Players.Heroes
{
    public class HeroesController : Controller, IHeroesController
    {
        private readonly AplicationContext _context;
        private readonly IResourceController _resourcesController;
        private readonly Random _random = new Random();

        public HeroesController(AplicationContext context, IResourceController resourcesController)
        {
            _context = context;
            _resourcesController = resourcesController;
        }

        [HttpPost]
        [Route("Heroes/RegistrationHero")]
        public async Task RegistrationHero(string heroId, int rare, string defaultViewId)
        {
            var template = new HeroTemplate
            {
                Id = heroId,
                Rare = (Rare)rare,
                DefaultViewId = defaultViewId
            };

            _context.Add(template);
            await _context.SaveChangesAsync();
        }

        [HttpPost]
        [Route("Heroes/LevelUp")]
        public async Task LevelUp(int playerId, int heroId)
        {
            var hero = await GetHero(playerId, heroId);
            
            if (hero == null)
                return;

            if (hero.PlayerId != playerId)
                return;
            
            await _context.SaveChangesAsync();
        }

        [HttpPost]
        [Route("Heroes/GetSimpleHeroes")]
        public async Task<List<Hero>> GetSimpleHeroes(int playerId, int count)
        {
            List<Hero> result = new List<Hero>();
            var cost = new Resource { PlayerId = playerId, Type = ResourceType.SimpleHireCard, Count = count, E10 = 0 };

            var allHeroes = await _context.HeroTemplates.ToListAsync();
            List<HeroTemplate> workList = new List<HeroTemplate>();
            HeroTemplate heroTemplate;

            await _resourcesController.SubstactResources(cost);

            for (int i = 0; i < count; i++)
            {
                var rand = _random.Next(0, 10001);
                if (rand < 5600f)
                {
                    workList = allHeroes.FindAll(x => (x.Rare == Rare.C));
                }
                else if (rand < 9000f)
                {
                    workList = allHeroes.FindAll(x => (x.Rare == Rare.UC));
                }
                else if (rand < 9850f)
                {
                    workList = allHeroes.FindAll(x => (x.Rare == Rare.R));
                }
                else if (rand < 9995f)
                {
                    workList = allHeroes.FindAll(x => (x.Rare == Rare.SR));
                }
                else if (rand <= 10000f)
                {
                    workList = allHeroes.FindAll(x => (x.Rare == Rare.SSR));
                }

                if (workList.Count > 0)
                {
                    heroTemplate = workList[_random.Next(0, workList.Count)];
                }
                else
                {
                    heroTemplate = allHeroes[_random.Next(0, allHeroes.Count)];
                }

                var hero = new Hero(playerId, heroTemplate);
                _context.Heroes.Add(hero);
                result.Add(hero);
            }
            await _context.SaveChangesAsync();
            return result;
        }

        [HttpPost]
        [Route("Heroes/GetSpecialHeroes")]
        public async Task<List<Hero>> GetSpecialHeroes(int playerId, int count)
        {
            List<Hero> result = new List<Hero>();

            var cost = new Resource { PlayerId = playerId, Type = ResourceType.SpecialHireCard, Count = count, E10 = 0 };
            await _resourcesController.SubstactResources(cost);

            var allHeroes = await _context.HeroTemplates.ToListAsync();
            List<HeroTemplate> workList = new List<HeroTemplate>();
            HeroTemplate heroTemplate;

            for (int i = 0; i < count; i++)
            {
                var rand = _random.Next(0, 10001);
                if (rand < 6000)
                {
                    workList = allHeroes.FindAll(x => (x.Rare == Rare.R));
                }
                else if (rand < 8842)
                {
                    workList = allHeroes.FindAll(x => (x.Rare == Rare.SR));
                }
                else if (rand < 9842)
                {
                    workList = allHeroes.FindAll(x => (x.Rare == Rare.SSR));
                }
                else if (rand <= 10000)
                {
                    workList = allHeroes.FindAll(x => (x.Rare == Rare.UR));
                }

                if (workList.Count > 0)
                {
                    heroTemplate = workList[_random.Next(0, workList.Count)];
                }
                else
                {
                    heroTemplate = allHeroes[_random.Next(0, allHeroes.Count)];
                }

                var hero = new Hero(playerId, heroTemplate);
                _context.Heroes.Add(hero);
                result.Add(hero);
            }

            await _context.SaveChangesAsync();
            return result;
        }

        public async Task<Hero> GetHero(int playerId, int heroId)
        {
            var hero = await _context.Heroes.FindAsync(heroId);
            return hero;
        }
    }
}
