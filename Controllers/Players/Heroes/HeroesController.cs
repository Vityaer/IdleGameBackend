﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Misc.Json;
using UniverseRift.Contexts;
using UniverseRift.Controllers.Common;
using UniverseRift.Controllers.Players.Inventories.Resources;
using UniverseRift.MessageData;
using UniverseRift.Misc;
using UniverseRift.Models.Heroes;
using UniverseRift.Models.Resources;
using UniverseRift.Models.Results;

namespace UniverseRift.Controllers.Players.Heroes
{
    public class HeroesController : Controller, IHeroesController
    {
        private readonly AplicationContext _context;
        private readonly IResourceController _resourcesController;
        private readonly IJsonConverter _jsonConverter;
        private readonly ICommonDictionaries _commonDictionaries;

        private readonly Random _random = new Random();

        public HeroesController(AplicationContext context, IJsonConverter jsonConverter, IResourceController resourcesController, ICommonDictionaries commonDictionaries)
        {
            _commonDictionaries = commonDictionaries;
            _jsonConverter = jsonConverter;
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
            if(checkResource == false)
                return answer;

            foreach ( var resource in cost)
            {
                //TODO: заменить на функцию для списка ресурсов
                await _resourcesController.SubstactResources(resource);
            }

            hero.Level += 1;
            await _context.SaveChangesAsync();
            answer.Result = "Success";
            return answer;
        }

        [HttpPost]
        [Route("Heroes/GetSimpleHeroes")]
        public async Task<AnswerModel> GetSimpleHeroes(int playerId, int count)
        {
            Console.WriteLine($"GetSimpleHeroes playerId: {playerId}");
            var answer = new AnswerModel();
            var heroesData = new List<HeroData>();

            List<Hero> heroes = new List<Hero>();
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
                await _context.SaveChangesAsync();

                var heroData = new HeroData(hero);
                heroesData.Add(heroData);
            }

            answer.Result = _jsonConverter.ToJson(heroesData);
            return answer;
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


        [HttpPost]
        [Route("Heroes/GetPlayerAllHeroes")]
        public async Task<AnswerModel> GetPlayerAllHeroes(int playerId)
        {
            var answer = new AnswerModel();

            var heroes = await _context.Heroes.ToListAsync();

            var playerHeroes = heroes.Where(hero => hero.PlayerId == playerId).ToList();

            answer.Result = _jsonConverter.ToJson(playerHeroes);
            return answer;
        }

        [HttpPost]
        [Route("Heroes/GetAllHeroes")]
        public async Task<AnswerModel> GetAllHeroes()
        {
            var answer = new AnswerModel();

            var heroes = await _context.Heroes.ToListAsync();

            answer.Result = _jsonConverter.ToJson(heroes);
            return answer;
        }
    }
}