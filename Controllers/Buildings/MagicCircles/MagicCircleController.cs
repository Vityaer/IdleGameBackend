using Microsoft.AspNetCore.Mvc;
using Misc.Json;
using UniverseRift.Contexts;
using UniverseRift.Controllers.Buildings.Achievments;
using UniverseRift.Controllers.Common;
using UniverseRift.GameModels.Heroes;
using UniverseRift.MessageData;
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
        private readonly IAchievmentController _achievmentController;

        private readonly Random _random = new Random();

        public MagicCircleController(
            AplicationContext context,
            IJsonConverter jsonConverter,
            IResourceManager resourcesController,
            ICommonDictionaries commonDictionaries,
            IAchievmentController achievmentController
            )
        {
            _commonDictionaries = commonDictionaries;
            _jsonConverter = jsonConverter;
            _context = context;
            _resourcesController = resourcesController;
            _achievmentController = achievmentController;
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
            var allHeroes = _commonDictionaries.Heroes;
            var workList = new List<HeroModel>();
            HeroModel heroTemplate;

            await _resourcesController.SubstactResources(cost);
            //вынести в json
            for (int i = 0; i < count; i++)
            {
                var rand = _random.Next(0, 10001);
                if (rand < 5600f)
                {
                    workList = allHeroes
                        .Where(x => (x.Value.General.Rare.Equals("C")))
                        .Select(x => x.Value)
                        .ToList();
                }
                else if (rand < 9000f)
                {
                    workList = allHeroes
                        .Where(x => (x.Value.General.Rare.Equals("C")))
                        .Select(x => x.Value)
                        .ToList();
                }
                else if (rand < 9850f)
                {
                    workList = allHeroes
                        .Where(x => (x.Value.General.Rare.Equals("C")))
                        .Select(x => x.Value)
                        .ToList();
                }
                else if (rand < 9995f)
                {
                    workList = allHeroes
                        .Where(x => (x.Value.General.Rare.Equals("C")))
                        .Select(x => x.Value)
                        .ToList();
                }
                else if (rand <= 10000f)
                {
                    workList = allHeroes
                        .Where(x => (x.Value.General.Rare.Equals("C")))
                        .Select(x => x.Value)
                        .ToList();
                }

                if (workList.Count > 0)
                {
                    heroTemplate = workList[_random.Next(0, workList.Count)];
                }
                else
                {
                    var heroCount = allHeroes.Count;
                    heroTemplate = allHeroes.ElementAt(_random.Next(0, heroCount)).Value;
                }

                var hero = new Hero(playerId, heroTemplate);
                _context.Heroes.Add(hero);
                await _context.SaveChangesAsync();

                var heroData = new HeroData(hero);
                heroesData.Add(heroData);
            }

            await _achievmentController.AchievmentUpdataData(playerId, "MagicCircleHireAchievment", count);
            answer.Result = _jsonConverter.Serialize(heroesData);
            return answer;
        }
    }
}
