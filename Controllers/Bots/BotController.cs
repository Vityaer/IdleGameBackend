using Microsoft.EntityFrameworkCore;
using Misc.Json;
using System;
using UniverseRift.Contexts;
using UniverseRift.Controllers.Buildings.Arenas;
using UniverseRift.Controllers.Common;
using UniverseRift.Controllers.Players;
using UniverseRift.Controllers.Players.Heroes;
using UniverseRift.GameModelDatas.AI;
using UniverseRift.GameModelDatas.Heroes;
using UniverseRift.MessageData;

namespace UniverseRift.Controllers.Bots
{
    public class BotController : IBotController
    {
        private const int BOT_COUNT = 10;

        private readonly AplicationContext _context;
        private readonly ICommonDictionaries _commonDictionaries;
        private readonly IPlayersController _playersController;
        private readonly IHeroesController _heroesController;
        private readonly IArenaController _arenaController;
        private readonly IJsonConverter _jsonConverter;

        private Random _random;

        public BotController(
            AplicationContext context,
            IJsonConverter jsonConverter,
            ICommonDictionaries commonDictionaries,
            IPlayersController playersController,
            IHeroesController heroesController,
            IArenaController arenaController
            )
        {
            _commonDictionaries = commonDictionaries;
            _playersController = playersController;
            _heroesController = heroesController;
            _arenaController = arenaController;
            _jsonConverter = jsonConverter;
            _context = context;
            _random = new();
        }

        public async Task OnStartServer()
        {
            for (var i = 0; i < BOT_COUNT; i++)
            {
                var name = $"Bot_{i}";
                var avatarIndex = _random.Next(_commonDictionaries.AvatarModels.Count);
                var avatarPath = _commonDictionaries.AvatarModels.ElementAt(avatarIndex).Value.Path;
                var player = await _playersController.CreatePlayer(name, avatarPath, true);

                if (player == null)
                    continue;

                player.IsBot = true;
                var botData = new BotData(player.Id);
                await _context.BotsDatas.AddAsync(botData);
                var heroesDataAnswer = await _heroesController.GetSimpleHeroes(player.Id, 10);

                var newHeroDatas = _jsonConverter.Deserialize<List<HeroData>>(heroesDataAnswer.Result);
                var arenaTeam = new TeamContainer();
                var heroCount = _random.Next(3, 7);
                for (var j = 0; j < heroCount; j++)
                {
                    var randomHeroIndex = _random.Next(newHeroDatas.Count);
                    arenaTeam.Heroes.Add(j, newHeroDatas[randomHeroIndex].Id);
                    newHeroDatas.RemoveAt(randomHeroIndex);
                }

                await _arenaController.SetDefenders(player.Id, _jsonConverter.Serialize(arenaTeam));
            }
        }
    }
}
