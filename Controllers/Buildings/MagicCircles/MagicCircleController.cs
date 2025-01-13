using Microsoft.AspNetCore.Mvc;
using Misc.Json;
using Models.City.MagicCircles;
using Models.Data.Inventories;
using UIController.Rewards.PosibleRewards;
using UniverseRift.Contexts;
using UniverseRift.Controllers.Buildings.Achievments;
using UniverseRift.Controllers.Common;
using UniverseRift.GameModels;
using UniverseRift.GameModels.Common;
using UniverseRift.Models.Resources;
using UniverseRift.Models.Results;
using UniverseRift.Services.Resources;
using UniverseRift.Services.Rewarders;

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
        private readonly IRewardService _clientRewardService;
        private readonly Random _random = new();

        public MagicCircleController(
            AplicationContext context,
            IJsonConverter jsonConverter,
            IResourceManager resourcesController,
            ICommonDictionaries commonDictionaries,
            IAchievmentController achievmentController,
            IRewardService clientRewardService
            )
        {
            _commonDictionaries = commonDictionaries;
            _jsonConverter = jsonConverter;
            _context = context;
            _resourcesController = resourcesController;
            _achievmentController = achievmentController;
            _clientRewardService = clientRewardService;
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

            if (!_commonDictionaries.Races.ContainsKey(raceName))
                raceName = _commonDictionaries.Races.ElementAt(0).Key;

            await _resourcesController.SubstactResources(playerCost);
            var rewardModel = new RewardModel();
            var posibleSplinters = new List<PosibleObjectData<SplinterData>>();
            foreach (var splinter in magicCircleBuildingModel.PosibleRewardData.Splinters)
            {
                var splinterModel = _commonDictionaries.Splinters[splinter.Value.Id];
                if (_commonDictionaries.Heroes.TryGetValue(splinterModel.ModelId, out var heroModel))
                {
                    if (heroModel.General.Race.Equals(raceName))
                    {
                        posibleSplinters.Add(splinter);
                    }
                }
            }

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

                if (selectBonus.Equals("Splinter") && posibleSplinters.Count == 0)
                {
                    selectBonus = "Resource";
                }

                switch (selectBonus)
                {
                    case "Resource":
                        var resource = GetRandomReward(magicCircleBuildingModel.PosibleRewardData.Resources);
                        if (resource != null)
                        {
                            var cloneResource = new ResourceData()
                            {
                                Type = resource.Type,
                                Amount = new BigDigit(resource.Amount.Mantissa, resource.Amount.E10)
                            };
                            rewardModel.Add(cloneResource);
                        }
                        break;
                    case "Item":
                        var item = GetRandomReward(magicCircleBuildingModel.PosibleRewardData.Items);
                        if (item != null)
                        {
                            var cloneItem = new ItemData()
                            {
                                Id = item.Id,
                                Amount = item.Amount,
                            };

                            rewardModel.Add(cloneItem);
                        }
                        break;
                    case "Splinter":
                        var splinter = GetRandomReward(posibleSplinters);

                        if (splinter != null)
                        {
                            var cloneSplinter = new SplinterData()
                            {
                                Id = splinter.Id,
                                Amount = splinter.Amount
                            };
                            rewardModel.Add(cloneSplinter);
                        }
                        break;
                }

            }
            await _achievmentController.AchievmentUpdataData(playerId, "MagicCircleHireAchievment", count);
            await _clientRewardService.AddReward(playerId, rewardModel);

            await _context.SaveChangesAsync();

            answer.Result = _jsonConverter.Serialize(rewardModel);
            return answer;
        }

        private T GetRandomReward<T>(List<PosibleObjectData<T>> subjects)
            where T : InventoryBaseItem
        {
            if (subjects.Count == 0)
                return null;

            if (subjects.Count == 1)
                return subjects[0].Value;

            var sum = 0f;
            foreach (var subject in subjects)
                sum += subject.Posibility;

            var index = -1;

            for (int i = 0; i < subjects.Count; i++)
            {
                var rand = (float)_random.NextDouble() * sum;
                var currentSum = rand;
                for (var j = 0; j < subjects.Count; j++)
                {
                    currentSum -= subjects[j].Posibility;
                    index += 1;
                    if (currentSum < 0f)
                        break;
                }
            }
            index = Math.Clamp(index, 0, subjects.Count - 1);

            return subjects[index].Value;
        }
    }
}
