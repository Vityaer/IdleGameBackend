using Cysharp.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Misc.Json;
using Models.City.Hires;
using UniRx;
using UniverseRift.Contexts;
using UniverseRift.Controllers.Common;
using UniverseRift.GameModelDatas.Players;
using UniverseRift.GameModels;
using UniverseRift.GameModels.Heroes;
using UniverseRift.MessageData;
using UniverseRift.Models.Achievments;
using UniverseRift.Models.Heroes;
using UniverseRift.Models.Resources;
using UniverseRift.Models.Results;
using UniverseRift.Models.Teams;
using UniverseRift.Services.Resources;

namespace UniverseRift.Controllers.Players.Heroes
{
    public class HeroesController : Controller, IHeroesController
    {
        private readonly AplicationContext _context;
        private readonly IResourceManager _resourcesController;
        private readonly IJsonConverter _jsonConverter;
        private readonly ICommonDictionaries _commonDictionaries;
        //private readonly IAchievmentController _achievmentController;

        private readonly Random _random = new();

        public HeroesController(
            AplicationContext context,
            IJsonConverter jsonConverter,
            IResourceManager resourcesController,
            ICommonDictionaries commonDictionaries)
        //IAchievmentController achievmentController)
        {
            _commonDictionaries = commonDictionaries;
            _jsonConverter = jsonConverter;
            _context = context;
            _resourcesController = resourcesController;
            //_achievmentController = achievmentController;
        }

        [HttpPost]
        [Route("Heroes/LevelUp")]
        public async Task<AnswerModel> LevelUp(int playerId, int heroId)
        {
            var answer = new AnswerModel();

            var hero = await GetHero(playerId, heroId);

            if (hero == null)
            {
                answer.Error = "Not found hero";
                return answer;
            }

            if (hero.PlayerId != playerId)
            {
                answer.Error = "wrong playerId";
                return answer;
            }
            var costContainer = _commonDictionaries.HeroesCostLevelUps["Heroes"];

            var cost = costContainer.GetCostForLevelUp(hero.Level, playerId);

            var checkResource = await _resourcesController.CheckResource(playerId, cost, answer);
            if (checkResource == false)
                return answer;

            await _resourcesController.SubstactResources(cost);

            hero.Level += 1;
            await _context.SaveChangesAsync();
            answer.Result = "Success";
            return answer;
        }

        [HttpPost]
        [Route("Heroes/RatingUp")]
        public async Task<AnswerModel> RatingUp(int playerId, int heroId, string heroesPaymentContainer)
        {
            var allHeroes = await _context.Heroes.ToListAsync();
            var answer = new AnswerModel();

            var hero = await GetHero(playerId, heroId);

            if (hero == null)
            {
                answer.Error = "Not found hero";
                return answer;
            }

            if (hero.PlayerId != playerId)
            {
                answer.Error = "wrong playerId";
                return answer;
            }

            var requires = _commonDictionaries.RatingUpContainers[$"{hero.Rating + 1}"];

            var cost = new List<Resource>();
            foreach (var resourseData in requires.Cost)
            {
                var resource = new Resource
                {
                    Type = resourseData.Type,
                    Count = resourseData.Amount.Mantissa,
                    E10 = resourseData.Amount.E10,
                    PlayerId = playerId
                };
                cost.Add(resource);
            }

            var checkResource = await _resourcesController.CheckResource(playerId, cost, answer);
            if (checkResource == false)
                return answer;

            await _resourcesController.SubstactResources(cost);

            var listHeroIds = _jsonConverter.Deserialize<List<int>>(heroesPaymentContainer);
            foreach (var id in listHeroIds)
            {
                var costHero = await _context.Heroes.FindAsync(id);

                if (costHero != null)
                    _context.Heroes.Remove(costHero);
            }

            hero.Rating += 1;
            await _context.SaveChangesAsync();
            answer.Result = "Success";

            allHeroes = await _context.Heroes.ToListAsync();
            return answer;
        }

        [HttpPost]
        [Route("Heroes/GetSimpleHeroes")]
        public async Task<AnswerModel> GetSimpleHeroes(int playerId, int count)
        {
            return await GetHireHeroes(playerId, count, "SimpleHire");
        }

        [HttpPost]
        [Route("Heroes/GetSpecialHeroes")]
        public async Task<AnswerModel> GetSpecialHeroes(int playerId, int count)
        {
            return await GetHireHeroes(playerId, count, "SpecialHire");
        }

        [HttpPost]
        [Route("Heroes/GetFriendHireHeroes")]
        public async Task<AnswerModel> GetFriendHireHeroes(int playerId, int count)
        {
            return await GetHireHeroes(playerId, count, "FriendHire");
        }

        private async Task<AnswerModel> GetHireHeroes(int playerId, int count, string hireType)
        {
            var answer = await GetHeroes(playerId, count, $"{hireType}Container");

            if (string.IsNullOrEmpty(answer.Error) && !string.IsNullOrEmpty(answer.Result))
            {
                await AchievmentUpdataData(playerId, $"{hireType}Achievment", count);
            }

            return answer;
        }

        private async Task<AnswerModel> GetHeroes(int playerId, int count, string hireContainerName)
        {
            var answer = new AnswerModel();
            var hireContainerModel = _commonDictionaries.HireContainerModels[hireContainerName];
            var costValue = hireContainerModel.Cost.Amount * count;
            var resourceData = new GameResource(hireContainerModel.Cost.Type, costValue);
            var playerCost = new Resource(playerId, resourceData);

            var permission = await _resourcesController.CheckResource(playerId, playerCost, answer);
            if (!permission)
                return answer;

            await _resourcesController.SubstactResources(playerCost);
            var heroesData = await CreateHeroes(playerId, count, hireContainerModel);
            answer.Result = _jsonConverter.Serialize(heroesData);
            return answer;
        }

        public async Task<List<HeroData>> CreateHeroes(int playerId, int count, HireContainerModel hireContainerModel)
        {
            var result = new List<HeroData>();

            var heroes = new List<Hero>();
            var allHeroes = _commonDictionaries.Heroes;
            var workList = new List<HeroModel>();
            HeroModel heroTemplate;

            var sum = 0f;
            foreach (var hireChance in hireContainerModel.ChanceHires)
                sum += hireChance.Chance;

            for (int i = 0; i < count; i++)
            {
                var rand = (float)_random.NextDouble() * sum;
                var index = -1;
                var currentSum = rand;
                for (var j = 0; j < hireContainerModel.ChanceHires.Count; j++)
                {
                    currentSum -= hireContainerModel.ChanceHires[j].Chance;
                    index += 1;
                    if (currentSum < 0f)
                        break;
                }

                index = Math.Clamp(index, 0, hireContainerModel.ChanceHires.Count);
                var selectHire = hireContainerModel.ChanceHires[index];
                workList = allHeroes
                    .Where(x => (x.Value.General.Rare == selectHire.Rare))
                    .Select(x => x.Value)
                    .ToList();

                if (workList.Count > 0)
                {
                    heroTemplate = workList[_random.Next(0, workList.Count)];
                }
                else
                {
                    heroTemplate = allHeroes.ElementAt(_random.Next(0, allHeroes.Count)).Value;
                }

                var hero = new Hero(playerId, heroTemplate);
                var rating = (int)selectHire.Rare;
                rating = Math.Clamp(rating, 1, 5);
                hero.Rating = rating;

                heroes.Add(hero);
            }

            await _context.Heroes.AddRangeAsync(heroes);
            await _context.SaveChangesAsync();

            foreach(var hero in heroes)
            {
				var heroData = new HeroData(hero);
				result.Add(heroData);
			}

            return result;
        }

        public async Task<Hero> GetHero(int playerId, int heroId)
        {
            var hero = await _context.Heroes.FindAsync(heroId);
            return hero;
        }


        [HttpPost]
        [Route("Heroes/GetPlayerAllHeroes")]
        public async Task<AnswerModel> GetPlayerAllHeroes(int playerId)
        {
            var answer = new AnswerModel();

            var heroes = await _context.Heroes.ToListAsync();

            var playerHeroes = heroes.Where(hero => hero.PlayerId == playerId).ToList();

            answer.Result = _jsonConverter.Serialize(playerHeroes);
            return answer;
        }

        [HttpPost]
        [Route("Heroes/GetAllHeroes")]
        public async Task<AnswerModel> GetAllHeroes()
        {
            var answer = new AnswerModel();

            var heroes = await _context.Heroes.ToListAsync();

            answer.Result = _jsonConverter.Serialize(heroes);
            return answer;
        }

		[HttpPost]
		[Route("Heroes/SetDefenders")]
		public async Task<AnswerModel> SetDefenders(int playerId, string heroesIdsContainer, string teamsContainerName)
		{
			var answer = new AnswerModel();
			var allTeamDatas = await _context.ServerPlayerTeamDatas.ToListAsync();

			var targetTeamData = allTeamDatas.Find(data => (data.PlayerId == playerId)
            && teamsContainerName.Equals(data.ArmyData));

            if (targetTeamData == null)
            {
                targetTeamData = new ServerPlayerTeamData(
                    playerId,
                    teamsContainerName,
                    heroesIdsContainer
                );

                await _context.ServerPlayerTeamDatas.AddAsync(targetTeamData);
            }
            else
            {
			    targetTeamData.ArmyData = heroesIdsContainer;
            }

			await _context.SaveChangesAsync();
			answer.Result = "Success!";
			return answer;
		}

		public async Task<HeroesStorage> GetPlayerSave(int playerId)
        {
            var result = new HeroesStorage();

            var heroes = await _context.Heroes.ToListAsync();

            var playerHeroes = heroes.FindAll(hero => hero.PlayerId == playerId);

            foreach (var hero in playerHeroes)
            {
                result.ListHeroes.Add(new HeroData(hero));
            }

            result.MaxCountHeroes = 100;

            return result;
        }

        private async Task AchievmentUpdataData(int playerId, string implementationName, int amount)
        {
            var allAchievments = await _context.AchievmentDatas.ToListAsync();
            var playerAchievments = allAchievments
                .FindAll(achievmentData => achievmentData.PlayerId == playerId);

            var models = _commonDictionaries.Achievments
                .Where(data => data.Value.ImplementationName.Equals(implementationName))
                .Select(data => data.Value);
            foreach (var achievmentModel in models)
            {
                var achievment = allAchievments.Find(
                    achievment => achievment.PlayerId == playerId
                    && achievment.ModelId == achievmentModel.Id);

                if (achievment == null)
                {
                    achievment = new AchievmentData(playerId, achievmentModel.Id);
                    await _context.AchievmentDatas.AddAsync(achievment);
                }
                achievment.Amount += amount;
            }

            await _context.SaveChangesAsync();
        }
    }
}
