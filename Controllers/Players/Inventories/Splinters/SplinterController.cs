using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Misc.Json;
using Models.Data.Inventories;
using UniverseRift.Contexts;
using UniverseRift.Controllers.Common;
using UniverseRift.Controllers.Players.Inventories.Items;
using UniverseRift.GameModels;
using UniverseRift.MessageData;
using UniverseRift.Models.Heroes;
using UniverseRift.Models.Inventories.Splinters;
using UniverseRift.Models.Results;

namespace UniverseRift.Controllers.Players.Inventories.Splinters
{
    public class SplinterController : ISplinterController
    {
        private readonly AplicationContext _context;
        private readonly ICommonDictionaries _commonDictionaries;
        private readonly IJsonConverter _jsonConverter;
        private readonly IItemsController _itemsController;

        public SplinterController(
            AplicationContext context,
            ICommonDictionaries commonDictionaries,
            IJsonConverter jsonConverter,
            IItemsController itemsController
            )
        {
            _jsonConverter = jsonConverter;
            _commonDictionaries = commonDictionaries;
            _itemsController = itemsController;
            _context = context;
        }

        public async Task<AnswerModel> AddSplinters(int playerId, string splinterId, int amount)
        {
            var answer = new AnswerModel();
            var allSplinters = await _context.Splinters.ToListAsync();
            var playerSplinters = allSplinters.FindAll(splinterData => splinterData.PlayerId == playerId);
            var selectedSplinter = playerSplinters.Find(splinterData => splinterData.SplinterId.Equals(splinterId));

            if (selectedSplinter == null)
            {
                selectedSplinter = new Splinter(playerId, splinterId, amount);
                await _context.Splinters.AddAsync(selectedSplinter);
            }
            else
            {
                selectedSplinter.Count += amount;
            }

            await _context.SaveChangesAsync();

            answer.Result = "Success";
            return answer;
        }

        [HttpPost]
        [Route("Cheats/UseSplinters")]
        public async Task<AnswerModel> UseSplinters(int playerId, string splinterId, int count)
        {
            var answer = new AnswerModel();

            var allSplinters = await _context.Splinters.ToListAsync();
            var playerSplinters = allSplinters.FindAll(splinterData => splinterData.PlayerId == playerId);
            var selectedSplinter = playerSplinters.Find(splinterData => splinterData.SplinterId.Equals(splinterId));

            if (selectedSplinter == null)
            {
                answer.Error = "Wrong data";
                return answer;
            }

            var model = _commonDictionaries.Splinters[splinterId];

            var requireCount = model.RequireCount * count;
            if (selectedSplinter.Count < requireCount)
            {
                answer.Error = "Wrong data";
                return answer;
            }

            selectedSplinter.Count -= requireCount;

            switch (model.SplinterType)
            {
                case SplinterType.Hero:
                    var allHeroes = _commonDictionaries.Heroes;
                    if (_commonDictionaries.Heroes.ContainsKey(model.ModelId))
                    {
                        var heroTemplate = allHeroes[model.ModelId];
                        var heroesData = new List<HeroData>();

                        var heroes = new List<Hero>(count);
                        for (var i = 0; i < count; i++)
                        {
                            var hero = new Hero(playerId, heroTemplate);
                            heroes.Add(hero);

                            var heroData = new HeroData(hero);
                            heroesData.Add(heroData);
                        }
                        await _context.Heroes.AddRangeAsync(heroes);
                        answer.Result = _jsonConverter.Serialize(heroesData);
                    }
                    break;
                case SplinterType.Item:
                    await _itemsController.AddItem(playerId, model.ModelId, count);
                    var itemData = new ItemData { Id = model.ModelId, Amount = count };
                    var rewardModel = new RewardModel();
                    rewardModel.Add(itemData);
                    answer.Result = _jsonConverter.Serialize(rewardModel);
                    break;
            }

            if (selectedSplinter.Count == 0)
            {
                _context.Splinters.Remove(selectedSplinter);
            }

            await _context.SaveChangesAsync();
            return answer;
        }
    }
}
