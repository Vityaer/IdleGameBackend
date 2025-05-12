using Microsoft.AspNetCore.Mvc;
using UniverseRift.Contexts;
using UniverseRift.Models.Heroes;
using UniverseRift.Models.Results;
using Misc.Json;
using UniverseRift.MessageData;
using UniverseRift.GameModels.Heroes;
using UniverseRift.Controllers.Common;
using Models.City.Sanctuaries;
using UniverseRift.GameModels;
using Models.Data.Inventories;
using UniverseRift.Services.Resources;
using UniverseRift.Models.Resources;
using System.Resources;

namespace UniverseRift.Controllers.Buildings
{
    public class SanctuaryController : ControllerBase
    {
        private readonly AplicationContext _context;
        private readonly IJsonConverter _jsonConverter;
        private static readonly Random _random = new Random();
        private readonly ICommonDictionaries _commonDictionaries;
		private readonly IResourceManager _resourcesController;

		public SanctuaryController(
            AplicationContext context,
            IJsonConverter jsonConverter,
            ICommonDictionaries commonDictionaries,
			IResourceManager resourcesController)
        {
            _context = context;
            _jsonConverter = jsonConverter;
            _commonDictionaries = commonDictionaries;
            _resourcesController = resourcesController;

		}

        [HttpPost]
        [Route("Sanctuary/ReplaceHero")]
        public async Task<AnswerModel> ReplaceHero(int playerId, int heroId, string targetRace)
        {
            var answer = new AnswerModel();

			if (!_commonDictionaries.Races.ContainsKey(targetRace) && (targetRace != "Random"))
			{
				answer.Error = "Race not found";
				return answer;
			}

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

			var sanctuaryBuildingModel = _commonDictionaries.Buildings[nameof(SanctuaryBuildingModel)] as SanctuaryBuildingModel;

			ResourceData costReplaceData = null;

            string raceResult = null;

			if (targetRace == "Random")
			{
                if ((hero.Rating - 1) >= sanctuaryBuildingModel.SimpleReplaceResource.Count)
                {
					answer.Error = "Error rating";
					return answer;
				}

				costReplaceData = sanctuaryBuildingModel.SimpleReplaceResource[hero.Rating - 1];

				var randomRaceIndex = _random.Next(0, _commonDictionaries.Races.Count);
				raceResult = _commonDictionaries.Races.ElementAt(randomRaceIndex).Value.Id;
			}
			else
			{
				if ((hero.Rating - 1) >= sanctuaryBuildingModel.ConcreteReplaceResource.Count)
				{
					answer.Error = "Error rating";
					return answer;
				}

				costReplaceData = sanctuaryBuildingModel.ConcreteReplaceResource[hero.Rating - 1];
                raceResult = targetRace;
			}

			var gameResource = new GameResource(costReplaceData);
			var resource = new Resource(playerId, gameResource);

			var resourceEnough = await _resourcesController.CheckResource(playerId, resource, answer);
			if (!resourceEnough)
			{
				answer.Error = "Wrong data";
				return answer;
			}

            await _resourcesController.SubstactResources(resource);

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
                .Where(template => template.Value.General.Race == raceResult)
                .Select(x => x.Value)
                .ToList();

            raceHeroes.Remove(oldHeroTemplate);

            var randomIndex = _random.Next(0, raceHeroes.Count);
            var newHero = new Hero(playerId, raceHeroes[randomIndex]);

            newHero.Rating = hero.Rating;

			_context.Heroes.Remove(hero);
            await _context.Heroes.AddAsync(newHero);
            await _context.SaveChangesAsync();

            var heroData = new HeroData(newHero);
            answer.Result = _jsonConverter.Serialize(heroData);
            return answer;
        }
    }
}
