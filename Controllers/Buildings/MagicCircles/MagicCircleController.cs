using Microsoft.AspNetCore.Mvc;
using Misc.Json;
using UniverseRift.Contexts;
using UniverseRift.Controllers.Buildings.Achievments;
using UniverseRift.Controllers.Common;
using UniverseRift.GameModelDatas.Cities.Buildings;
using UniverseRift.GameModels;
using UniverseRift.Models.Resources;
using UniverseRift.Models.Results;
using UniverseRift.Services.Resources;

namespace UniverseRift.Controllers.Buildings.MagicCircles
{
    public class MagicCircleController : Controller
    {
        private const string MAGIC_CIRCLE_NAME = "MagicCircleBuildingModel";

        private readonly AplicationContext _context;
        private readonly IResourceManager _resourcesController;
        private readonly IJsonConverter _jsonConverter;
        private readonly ICommonDictionaries _commonDictionaries;
        private readonly IAchievmentController _achievmentController;
        private readonly Random _random = new();

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
        public async Task<AnswerModel> GetHeroes(int playerId, int count, string raceName)
        {
            var answer = new AnswerModel();
            var magicCircleBuildingModel = _commonDictionaries.Buildings[MAGIC_CIRCLE_NAME] as MagicCircleBuildingModel;

            var costValue = magicCircleBuildingModel.HireCost.Amount * count;
            var resourceData = new GameResource(magicCircleBuildingModel.HireCost.Type, costValue);
            var playerCost = new Resource(playerId, resourceData);

            var permission = await _resourcesController.CheckResource(playerId, playerCost, answer);
            if (!permission)
                return answer;

            await _resourcesController.SubstactResources(playerCost);

            var sum = 0f;
            foreach (var hireChance in magicCircleBuildingModel.SubjectChances)
                sum += hireChance.Value;

            for (int i = 0; i < count; i++)
            {
                var rand = (float)_random.NextDouble() * sum;
                var index = -1;
                var currentSum = rand;
                for (var j = 0; j < magicCircleBuildingModel.SubjectChances.Count; j++)
                {
                    currentSum -= magicCircleBuildingModel.SubjectChances.ElementAt(j).Value;
                    index += 1;
                    if (currentSum < 0f)
                        break;
                }

                index = Math.Clamp(index, 0, magicCircleBuildingModel.SubjectChances.Count);
                var selectBonus = magicCircleBuildingModel.SubjectChances.ElementAt(index).Key;
                switch (selectBonus)
                {
                    case "Item":
                        var itemId = GetRandomSubject(magicCircleBuildingModel.Items);
                        break;
                    case "Splinter":
                        var splinterId = GetRandomSubject(magicCircleBuildingModel.Splinters);
                        break;
                }

            }

            await _achievmentController.AchievmentUpdataData(playerId, "MagicCircleHireAchievment", count);
            answer.Result = _jsonConverter.Serialize(heroesData);
            return answer;
        }

        private string GetRandomSubject(Dictionary<string, float> subjects)
        {
            if (subjects.Count == 0)
                return string.Empty;

            var sum = 0f;
            foreach (var subject in subjects)
                sum += subject.Value;

            var index = -1;

            for (int i = 0; i < subjects.Count; i++)
            {
                var rand = (float)_random.NextDouble() * sum;
                var currentSum = rand;
                for (var j = 0; j < subjects.Count; j++)
                {
                    currentSum -= subjects.ElementAt(j).Value;
                    index += 1;
                    if (currentSum < 0f)
                        break;
                }
            }
            index = Math.Clamp(index, 0, subjects.Count - 1);

            return subjects.ElementAt(index).Key;

        }
    }
}
