using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Misc.Json;
using Models.City.Arena;
using Models.City.Sanctuaries;
using UniverseRift.Contexts;
using UniverseRift.Controllers.Buildings.Achievments;
using UniverseRift.Controllers.Common;
using UniverseRift.GameModelDatas.Cities.Buildings;
using UniverseRift.GameModelDatas.Heroes;
using UniverseRift.GameModelDatas.Players;
using UniverseRift.GameModels;
using UniverseRift.Heplers.Utils;
using UniverseRift.MessageData;
using UniverseRift.Models.Arenas;
using UniverseRift.Models.Misc;
using UniverseRift.Models.Misc.Communications;
using UniverseRift.Models.Results;
using UniverseRift.Services.Rewarders;

namespace UniverseRift.Controllers.Buildings.Arenas
{
    public class ArenaController : ControllerBase, IArenaController
    {
        private const int AMOUNT_UP_DELTA = 100;
        private const int AMOUNT_DOWN_DELTA = 50;
        private const int DELTA_BET_STEP = 50;
        private const int EQUALS_DELTA = 100;

        private readonly AplicationContext _context;
        private readonly IJsonConverter m_jsonConverter;
        private readonly IAchievmentController _achievmentController;
        private readonly ICommonDictionaries m_commonDictionaries;
        private readonly IRewardService m_rewardService;

		private static readonly Random _random = new();

        public ArenaController(
            AplicationContext context,
            IJsonConverter jsonConverter,
            IAchievmentController achievmentController,
			ICommonDictionaries commonDictionaries
			)
        {
            _achievmentController = achievmentController;
            _context = context;
            m_jsonConverter = jsonConverter;
			m_commonDictionaries = commonDictionaries;

		}

		public async Task OnPlayerRegister(int playerId)
		{
			var arenaFighters = await _context.ServerArenaPlayerDatas.ToListAsync();
			var playerData = arenaFighters.Find(data => data.PlayerId == playerId);

			if (playerData == null)
			{
				playerData = new ServerArenaPlayerData()
				{
					PlayerId = playerId,
					Score = 1000,
					ArmyData = string.Empty,
				};

				await _context.ServerArenaPlayerDatas.AddAsync(playerData);
				await _context.SaveChangesAsync();
			}
		}

		public async Task OnStartServer()
		{
            var allArenaContainer = await _context.ArenaSeasons.ToListAsync();
            var simpleArena = allArenaContainer.Find(arena => arena.ArenaType == ArenaType.Simple);
            
            if (simpleArena == null)
            {
				var now = DateTime.UtcNow;
				simpleArena = new ArenaSeason(ArenaType.Simple, now.ToString());

                await _context.ArenaSeasons.AddAsync(simpleArena);
                await _context.SaveChangesAsync();
			}
		}

		public async Task RefreshDay()
		{
			var allArenaContainer = await _context.ArenaSeasons.ToListAsync();
			var simpleArena = allArenaContainer.Find(arena => arena.ArenaType == ArenaType.Simple);

            if (simpleArena == null)
                return;

			var arenaBuildingModel = m_commonDictionaries.Buildings[nameof(ArenaBuildingModel)] as ArenaBuildingModel;

            var dateTimeStart = DateTimeUtils.TryParseOrNow(simpleArena.StartDateTime);
            var now = DateTime.UtcNow;

            var distance = now - dateTimeStart;

            var simpleArenaModel = arenaBuildingModel.ArenaContainers[ArenaType.Simple];

			if (distance.TotalHours >= simpleArenaModel.WorkHours)
            {
				var allArenaFighters = await _context.ServerArenaPlayerDatas.ToListAsync();
				allArenaFighters.Sort(new ArenaFighterComparer());

				for (var i = 0; i < allArenaFighters.Count; i++)
				{
                    var arenaRewardModel = simpleArenaModel.RewardModels.First(reward => (i + 1) < reward.PositionMax);

                    var rewardLetter = new LetterData
                    {
                        ReceiverPlayerId = allArenaFighters[i].PlayerId,
                        RewardJSON = m_jsonConverter.Serialize(arenaRewardModel.Reward),
						IsOpened = false,
						IsRewardReceived = false,
						CreateDateTime = now.ToString(),
						Topic = "SimpleArenaRewardTopic",
                        Message = "SimpleArenaRewardMessage",
                        SenderPlayerId = -1,
					};

                    allArenaFighters[i].Refresh();
				}

				_context.ArenaSeasons.Remove(simpleArena);

				now = DateTime.UtcNow;
				simpleArena = new ArenaSeason(ArenaType.Simple, now.ToString());

				await _context.ArenaSeasons.AddAsync(simpleArena);

				await _context.SaveChangesAsync();
			}
		}

		public async Task<ArenaData> GetPlayerSave(int playerId, CommunicationData communicationData)
        {
            var result = new ArenaData();
            var arenaFighters = await _context.ServerArenaPlayerDatas.ToListAsync();
            var playerData = arenaFighters.Find(data => data.PlayerId == playerId);

            if (playerData == null)
            {
                playerData = new ServerArenaPlayerData()
                {
                    PlayerId = playerId,
                    Score = 1000,
                    ArmyData = string.Empty,
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

                var team = m_jsonConverter.Deserialize<TeamContainer>(opponent.ArmyData);
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

                var teamContainer = m_jsonConverter.Deserialize<TeamContainer>(arenaFighter.ArmyData);
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

			var allArenaContainer = await _context.ArenaSeasons.ToListAsync();
			var simpleArena = allArenaContainer.Find(arena => arena.ArenaType == ArenaType.Simple);

            result.ArenaGeneralData.SimpleArenaDateTimeStartText = simpleArena.StartDateTime;

            if (!string.IsNullOrEmpty(playerData.ArmyData))
			    result.MyData.Team = m_jsonConverter.Deserialize<TeamContainer>(playerData.ArmyData);

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

                if (opponentData.Score > opponentData.MaxScore)
                {
                    opponentData.MaxScore = opponentData.Score;
                }
            }
            else
            {
                playerData.Score += bet;
                opponentData.Score -= bet;

				if (playerData.Score > playerData.MaxScore)
				{
					playerData.MaxScore = playerData.Score;
				}
			}

            await _context.SaveChangesAsync();
            CommunicationData communicationData = new();
            var newData = await GetPlayerSave(playerId, communicationData);
            newData.PlayersData = communicationData.PlayersData;

            await _achievmentController.AchievmentUpdataData(playerId, "ArenaTryFightAchievment", 1);

            answer.Result = m_jsonConverter.Serialize(newData);
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
                };

                await _context.ServerArenaPlayerDatas.AddAsync(playerData);
            }
            else
            {
                playerData.ArmyData = heroesIdsContainer;
            }

			await _context.SaveChangesAsync();
			answer.Result = "Success!";
            return answer;
        }
	}
}
