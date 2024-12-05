using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Misc.Json;
using UniverseRift.Contexts;
using UniverseRift.Controllers.Buildings.Achievments;
using UniverseRift.GameModelDatas.Cities.Buildings;
using UniverseRift.GameModelDatas.Heroes;
using UniverseRift.GameModelDatas.Players;
using UniverseRift.GameModels;
using UniverseRift.MessageData;
using UniverseRift.Models.Arenas;
using UniverseRift.Models.Misc;
using UniverseRift.Models.Results;

namespace UniverseRift.Controllers.Buildings.Arenas
{
    public class ArenaController : ControllerBase, IArenaController
    {
        private const int AMOUNT_UP_DELTA = 100;
        private const int AMOUNT_DOWN_DELTA = 50;
        private const int DELTA_BET_STEP = 50;
        private const int EQUALS_DELTA = 100;

        private readonly AplicationContext _context;
        private readonly IJsonConverter _jsonConverter;
        private readonly IAchievmentController _achievmentController;

        private Random _random;

        public ArenaController(
            AplicationContext context,
            IJsonConverter jsonConverter,
            IAchievmentController achievmentController

            )
        {
            _achievmentController = achievmentController;
            _context = context;
            _random = new Random();
            _jsonConverter = jsonConverter;
        }

        public async Task<ArenaBuildingModel> GetPlayerSave(int playerId, CommunicationData communicationData)
        {
            var result = new ArenaBuildingModel();
            var arenaFighters = await _context.ServerArenaPlayerDatas.ToListAsync();
            var playerData = arenaFighters.Find(data => data.PlayerId == playerId);

            if (playerData == null)
            {
                playerData = new ServerArenaPlayerData()
                {
                    PlayerId = playerId,
                    Score = 1000,
                    CurrentAreanaLevel = 0,
                };

                await _context.ServerArenaPlayerDatas.AddAsync(playerData);
                await _context.SaveChangesAsync();
            }

            arenaFighters.Remove(playerData);

            List<int> weaknessOpponentIds = new();
            List<int> equalsOpponentIds = new();
            List<int> strongnessOpponentIds = new();

            foreach (var opponent in arenaFighters)
            {
                if (string.IsNullOrEmpty(opponent.ArmyData))
                    continue;

                var team = _jsonConverter.Deserialize<TeamContainer>(opponent.ArmyData);
                if(team.Heroes.Count == 0)
                    continue;

                var delta = Math.Abs(opponent.Score - playerData.Score);

                if (delta < EQUALS_DELTA)
                {
                    equalsOpponentIds.Add(opponent.PlayerId);
                }
                else if (opponent.Score < playerData.Score)
                {
                    weaknessOpponentIds.Add(opponent.PlayerId);
                }
                else
                {
                    strongnessOpponentIds.Add(opponent.PlayerId);
                }
            }

            List<int> opponentIds = new(3);
            AddOpponent(opponentIds, strongnessOpponentIds, 1);
            AddOpponent(opponentIds, weaknessOpponentIds, 1);
            AddOpponent(opponentIds, equalsOpponentIds, 1);

            if (result.Opponents.Count < 3)
            {
                var lostopponentIds = new List<int>();
                lostopponentIds.AddRange(strongnessOpponentIds);
                lostopponentIds.AddRange(equalsOpponentIds);
                lostopponentIds.AddRange(weaknessOpponentIds);

                AddOpponent(opponentIds, lostopponentIds, 3 - opponentIds.Count);
            }

            foreach (var id in opponentIds)
            {
                var serverOpponent = arenaFighters.Find(fighter => fighter.PlayerId == id);

                if (serverOpponent == null)
                    continue;

                result.Opponents.Add(new ArenaPlayerData(serverOpponent));
            }

            var heroes = await _context.Heroes.ToListAsync();
            foreach(var opponent in result.Opponents)
            {
                opponent.Mission = new();
                opponent.Mission.Units = new();

                var arenaFighter = arenaFighters.Find(player => player.PlayerId == opponent.PlayerId);
                
                if(arenaFighter == null)
                    continue;

                var teamContainer = _jsonConverter.Deserialize<TeamContainer>(arenaFighter.ArmyData);
                foreach (var teamHero in teamContainer.Heroes)
                {
                    var hero = heroes.Find(hero => hero.Id == teamHero.Value);
                    if (hero == null)
                        continue;

                    var heroData = new HeroData(hero);
                    opponent.Mission.Units.Add(heroData);
                }
            }

            if (communicationData != null)
            {
                foreach (var opponent in result.Opponents)
                {
                    var opponentData = await _context.Players.FindAsync(opponent.PlayerId);
                    if(opponentData == null)
                        continue;

                    communicationData.AddPlayerData(new PlayerData(opponentData));
                }
            }

            result.MyData = new ArenaPlayerData(playerData);
            result.MyData.Team = _jsonConverter.Deserialize<TeamContainer>(playerData.ArmyData);
            return result;
        }

        private void AddOpponent(List<int> opponents, List<int> serverOpponents, int count)
        {
            for (var i = 0; i < count; i++)
            {
                if (serverOpponents.Count == 0)
                    return;

                var randomIndex = _random.Next(serverOpponents.Count);
                opponents.Add(serverOpponents[randomIndex]);
                serverOpponents.RemoveAt(randomIndex);
            }
        }

        [HttpPost]
        [Route("Arena/FinishFight")]
        public async Task<AnswerModel> FinishFight(int playerId, int opponentId, int result)
        {
            var answer = new AnswerModel();

            var arenaFighters = await _context.ServerArenaPlayerDatas.ToListAsync();

            var playerData = arenaFighters.Find(data => data.PlayerId == playerId);
            var opponentData = arenaFighters.Find(data => data.PlayerId == opponentId);

            if (playerData == null || opponentData == null)
            {
                answer.Error = "Player not found";
                return answer;
            }

            var bet = 25;
            if (result == 0)
            {
                playerData.Score -= bet;
                opponentData.Score += bet;
            }
            else
            {
                playerData.Score += bet;
                opponentData.Score -= bet;
            }

            await _context.SaveChangesAsync();
            CommunicationData communicationData = new();
            var newData = await GetPlayerSave(playerId, communicationData);
            newData.PlayersData = communicationData.PlayersData;

            await _achievmentController.AchievmentUpdataData(playerId, "ArenaTryFightAchievment", 1);

            answer.Result = _jsonConverter.Serialize(newData);
            return answer;
        }

        [HttpPost]
        [Route("Arena/SetDefenders")]
        public async Task<AnswerModel> SetDefenders(int playerId, string heroesIdsContainer)
        {
            var answer = new AnswerModel();
            var arenaFighters = await _context.ServerArenaPlayerDatas.ToListAsync();

            var playerData = arenaFighters.Find(data => data.PlayerId == playerId);

            if (playerData == null)
            {
                playerData = new ServerArenaPlayerData()
                {
                    PlayerId = playerId,
                    Score = 1000,
                    CurrentAreanaLevel = 0,
                };

                await _context.ServerArenaPlayerDatas.AddAsync(playerData);
            }

            playerData.ArmyData = heroesIdsContainer;

            answer.Result = "Success!";
            return answer;
        }
    }
}
