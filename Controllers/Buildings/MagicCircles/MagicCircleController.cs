using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Misc.Json;
using System;
using UniverseRift.Contexts;
using UniverseRift.Controllers.Common;
using UniverseRift.MessageData;
using UniverseRift.Misc;
using UniverseRift.Models.Heroes;
using UniverseRift.Models.Resources;
using UniverseRift.Models.Results;
using UniverseRift.Services.Resources;

namespace UniverseRift.Controllers.Buildings.MagicCircles
{
    public class MagicCircleController : Controller
    {
        private readonly AplicationContext _context;
        private readonly IResourceManager _resourcesController;
        private readonly IJsonConverter _jsonConverter;
        private readonly ICommonDictionaries _commonDictionaries;

        private readonly Random _random = new Random();

        public MagicCircleController(AplicationContext context, IJsonConverter jsonConverter, IResourceManager resourcesController, ICommonDictionaries commonDictionaries)
        {
            _commonDictionaries = commonDictionaries;
            _jsonConverter = jsonConverter;
            _context = context;
            _resourcesController = resourcesController;
        }

        [HttpPost]
        [Route("MagicCircle/GetHeroes")]
        public async Task<AnswerModel> GetHeroes(int playerId, int count)
        {
            var answer = new AnswerModel();

            var cost = new Resource { PlayerId = playerId, Type = ResourceType.RaceHireCard, Count = count, E10 = 0 };

            var permission = await _resourcesController.CheckResource(playerId, cost, answer);
            if (!permission)
            {
                return answer;
            }

            var heroesData = new List<HeroData>();
            var heroes = new List<Hero>();
            var allHeroes = await _context.HeroTemplates.ToListAsync();
            var workList = new List<HeroTemplate>();
            HeroTemplate heroTemplate;

            await _resourcesController.SubstactResources(cost);
            //вынести в json
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
                await _context.SaveChangesAsync();

                var heroData = new HeroData(hero);
                heroesData.Add(heroData);
            }

            answer.Result = _jsonConverter.Serialize(heroesData);
            return answer;
        }
    }
}
