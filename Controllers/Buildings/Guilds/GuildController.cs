using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Misc.Json;
using Models.City.Guilds;
using Models.City.Hires;
using UniverseRift.Contexts;
using UniverseRift.Controllers.Common;
using UniverseRift.Controllers.Services.TaskCreators;
using UniverseRift.GameModelDatas.Players;
using UniverseRift.GameModels;
using UniverseRift.GameModels.Common;
using UniverseRift.Heplers.GameLogging;
using UniverseRift.Heplers.Utils;
using UniverseRift.Models.Guilds;
using UniverseRift.Models.Misc;
using UniverseRift.Models.Resources;
using UniverseRift.Models.Results;
using UniverseRift.Models.Tasks;
using UniverseRift.Models.Tasks.SimpleTask;
using UniverseRift.Services.Resources;
using UniverseRift.Services.Rewarders;

namespace UniverseRift.Controllers.Buildings.Guilds
{
	public class GuildController : ControllerBase, IGuildController
	{
		private const int BOSS_FREE_RAID_COUNT = 2;
		private const int BOSS_RAID_REFRESH_HOURS = 16;
		private const int BOSS_RAID_COST_STEP = 50;
		private const int GUILD_PAGGINATION_COUNT = 10;
		private const string DONATE_EVOLUTION_COST_CONTAINER_KEY = "GuildDonateEvolution";

		private readonly static Random _random = new Random();
		private readonly AplicationContext _context;
		private readonly ICommonDictionaries _commonDictionaries;
		private readonly IRewardService _clientRewardService;
		private readonly IJsonConverter _jsonConverter;
		private readonly IResourceManager _resourcesController;
		private readonly ITaskCreatorService _taskCreatorService;
		public GuildController(
			AplicationContext context,
			ICommonDictionaries commonDictionaries,
			IJsonConverter jsonConverter,
			IRewardService clientRewardService,
			IResourceManager resourcesController,
			ITaskCreatorService taskCreatorService
			)
		{
			_context = context;
			_clientRewardService = clientRewardService;
			_commonDictionaries = commonDictionaries;
			_resourcesController = resourcesController;
			_jsonConverter = jsonConverter;
			_taskCreatorService = taskCreatorService;

		}

		public async Task OnPlayerRegister(int playerId)
		{
			var recruit = new RecruitData { PlayerId = playerId };
			await _context.RecruitDatas.AddAsync(recruit);
			await _context.SaveChangesAsync();
		}

		public async Task RefreshDay()
		{
			var allRecruits = await _context.RecruitDatas.ToListAsync();
			foreach (var recruit in allRecruits)
				recruit.RefreshDay();

			await _context.SaveChangesAsync();
		}

		public async Task<GuildPlayerSaveContainer> GetPlayerSave(int playerId, CommunicationData communicationData, bool flagCreateNewData)
		{
			var result = new GuildPlayerSaveContainer();

			var player = await _context.Players.FindAsync(playerId);

			var allRecruits = await _context.RecruitDatas.ToListAsync();

			var recruit = await _context.RecruitDatas.FindAsync(playerId);


			if (recruit != null)
				await CheckRefreshRecruitRaids(recruit);

			if (player == null || player.GuildId == -1)
			{
				return result;
			}

			var guildRecruits = allRecruits.FindAll(recruit => recruit.GuildId == player.GuildId);
			var guildData = await _context.GuildDatas.FindAsync(player.GuildId);
			if (guildData != null)
			{
				result.GuildData = guildData;
				if (guildData.LeaderId == player.GuildId)
				{
					var requests = await _context.GuildPlayerRequests.ToListAsync();
					result.Requests = requests.FindAll(request => request.GuildId == player.GuildId);

					var allPlayers = await _context.Players.ToListAsync();

					foreach (var request in result.Requests)
					{
						var recruitPlayer = allPlayers.Find(player => player.Id == request.PlayerId);
						if (recruitPlayer == null)
							continue;

						communicationData.AddPlayerData(new PlayerData(recruitPlayer));
					}
				}

				result.GuildRecruits = guildRecruits;

				var allTasks = await _context.GameTasks.ToListAsync();

				List<GameTask> playerTasks = new();

				var playerAllTasks = allTasks.FindAll(task => task.PlayerId == playerId);
				foreach (var task in playerAllTasks)
				{
					if (_commonDictionaries.GameTaskModels.TryGetValue(task.TaskModelId, out var taskModel))
					{
						if (taskModel.SourceType == GameTaskSourceType.Guild)
						{
							playerTasks.Add(task);
						}
					}
				}

				if (flagCreateNewData)
				{
					playerTasks = await RefreshTasks(playerTasks, guildData, playerId);
				}

				foreach (var task in playerTasks)
				{
					result.TasksData.ListTasks.Add(new TaskData(task));
				}
			}


			return result;
		}

		private async Task<List<GameTask>> RefreshTasks(List<GameTask> playerTasks, GuildData guildData, int playerId)
		{
			List<GameTask> result = new();

			GuildBuildingModel guildBuilding = _commonDictionaries.Buildings[nameof(GuildBuildingModel)] as GuildBuildingModel;

			int levelIndexContainer = Math.Clamp(guildData.Level, 0, guildBuilding.LevelContainers.Count);

			GuildLevelContainer levelContainer = guildBuilding.LevelContainers[levelIndexContainer];

			int taskCount = levelContainer.DailyTaskCount;

			var notStartableTasks = playerTasks.FindAll(task => task.Status == TaskStatusType.NotStart).ToArray();

			_context.GameTasks.RemoveRange(notStartableTasks);

			if (notStartableTasks.Length < taskCount)
			{
				var notEnoughCount = taskCount - notStartableTasks.Length;
				GetNewTasks(playerId, notEnoughCount, GameTaskSourceType.Guild, levelContainer.TaskPosibilityContainers, out var newTasks);

				await _context.GameTasks.AddRangeAsync(newTasks);
				await _context.SaveChangesAsync();

				result = newTasks;
			}

			return result;
		}

		[HttpPost]
		[Route("Guild/CreateNewGuild")]
		public async Task<AnswerModel> CreateNewGuild(int playerId, string guildName, string iconPath)
		{
			var answer = new AnswerModel();
			var player = await _context.Players.FindAsync(playerId);

			if (player == null)
			{
				answer.Error = "Wrong data: player null";
				return answer;
			}

			if (player.GuildId >= 0)
			{
				answer.Error = $"Wrong data: player.GuildId: {player.GuildId}";
				return answer;
			}

			var newGuild = new GuildData { LeaderId = playerId, Name = guildName, IconPath = "123" };
			var container = _commonDictionaries.GuildBossContainers["MainBosses"];
			var bossData = container.Missions[newGuild.CurrentBoss].BossModels[0];
			newGuild.BossHealthMantissa = bossData.Health.Mantissa;
			newGuild.BossHealthE10 = bossData.Health.E10;

			await _context.GuildDatas.AddAsync(newGuild);
			await _context.SaveChangesAsync();

			player.GuildId = newGuild.Id;


			var recruit = await _context.RecruitDatas.FindAsync(playerId);
			if (recruit != null)
			{
				recruit.GuildId = newGuild.Id;
			}

			var newTasks = await RefreshTasks(new List<GameTask>(), newGuild, playerId);
			var guildPlayerSaveContainer = new GuildPlayerSaveContainer(newGuild, new List<RecruitData> { recruit }, newTasks);
			answer.Result = _jsonConverter.Serialize(guildPlayerSaveContainer);

			await _context.SaveChangesAsync();
			return answer;
		}

		[HttpPost]
		[Route("Guild/GetAvailableGuilds")]
		public async Task<AnswerModel> GetAvailableGuilds(int playerId, int pagginationIndex)
		{
			var answer = new AnswerModel();
			var player = await _context.Players.FindAsync(playerId);

			if (player == null || player.GuildId > 0 || pagginationIndex < 0)
			{
				answer.Error = "Wrong data";
				return answer;
			}

			var guilds = await _context.GuildDatas.ToListAsync();

			var leftIndex = pagginationIndex * GUILD_PAGGINATION_COUNT;

			if (guilds.Count == 0)
				return answer;

			if (leftIndex >= guilds.Count)
			{
				answer.Error = "Wrong data";
				return answer;
			}

			var rightIndex = Math.Clamp(leftIndex + GUILD_PAGGINATION_COUNT, leftIndex, guilds.Count - 1);

			var count = rightIndex - leftIndex + 1;
			var selectedGuilds = new GuildData[count];

			guilds.CopyTo(leftIndex, selectedGuilds, 0, count);

			answer.Result = _jsonConverter.Serialize(selectedGuilds.ToList());
			return answer;
		}

		[HttpPost]
		[Route("Guild/CreatePlayerRequest")]
		public async Task<AnswerModel> CreatePlayerRequest(int playerId, int guildId)
		{
			var answer = new AnswerModel();

			var player = await _context.Players.FindAsync(playerId);
			if (player == null || player.GuildId > 0)
			{
				answer.Error = "Wrong data";
				return answer;
			}

			var bans = await _context.GuildPlayerBans.ToListAsync();
			var similarBan = bans.Find(ban => ban.PlayerId == playerId && ban.GuildId == guildId);
			if (similarBan != null)
			{
				answer.Error = "This request failed";
				return answer;
			}

			var requests = await _context.GuildPlayerRequests.ToListAsync();
			var similarRequest = requests.Find(request => request.PlayerId == playerId && request.GuildId == guildId);

			if (similarRequest != null)
			{
				answer.Error = "You have already applied";
				return answer;
			}

			var guild = await _context.GuildDatas.FindAsync(guildId);
			if (guild == null)
			{
				answer.Error = "Wrong data";
				return answer;
			}

			var request = new GuildPlayerRequest
			{
				PlayerId = playerId,
				GuildId = guildId,
				CreateDate = DateTime.UtcNow.ToString(Constants.Common.DateTimeFormat)
			};

			await _context.GuildPlayerRequests.AddAsync(request);
			await _context.SaveChangesAsync();

			answer.Result = "Success";
			return answer;
		}

		[HttpPost]
		[Route("Guild/RemovePlayerRequest")]
		public async Task<AnswerModel> RemovePlayerRequest(int playerId, int requestId)
		{
			var answer = new AnswerModel();

			var request = await _context.GuildPlayerRequests.FindAsync(requestId);
			if (request == null || request.PlayerId != playerId)
			{
				answer.Error = "Wrong data";
				return answer;
			}

			_context.GuildPlayerRequests.Remove(request);
			await _context.SaveChangesAsync();

			answer.Result = "Success";
			return answer;
		}

		[HttpPost]
		[Route("Guild/GetPlayerRequests")]
		public async Task<AnswerModel> GetPlayerRequests(int playerId, int guildId)
		{
			var answer = new AnswerModel();

			var player = await _context.Players.FindAsync(playerId);
			if (player == null || player.GuildId != guildId)
			{
				answer.Error = "Wrong data";
				return answer;
			}


			var guild = await _context.GuildDatas.FindAsync(guildId);
			if (guild == null || guild.LeaderId != playerId)
			{
				answer.Error = "Wrong data";
				return answer;
			}

			var requests = await _context.GuildPlayerRequests.ToListAsync();
			var suitableRequests = requests.FindAll(request => request.GuildId == guildId);

			answer.Result = _jsonConverter.Serialize(suitableRequests);
			return answer;
		}

		[HttpPost]
		[Route("Guild/GetPlayersFromGuild")]
		public async Task<AnswerModel> GetPlayersFromGuild(int playerId, int guildId)
		{
			var answer = new AnswerModel();

			var player = await _context.Players.FindAsync(playerId);
			if (player == null || player.GuildId != guildId)
			{
				answer.Error = "Wrong data";
				return answer;
			}

			var guild = await _context.GuildDatas.FindAsync(guildId);
			if (guild == null)
			{
				answer.Error = "Wrong data";
				return answer;
			}

			var players = await _context.Players.ToListAsync();
			var playerFromGuild = players.FindAll(player => player.GuildId == guildId);

			answer.Result = _jsonConverter.Serialize(playerFromGuild);
			return answer;
		}

		[HttpPost]
		[Route("Guild/ApplyRequest")]
		public async Task<AnswerModel> ApplyRequest(int playerId, int guildId, int requestId)
		{
			var answer = new AnswerModel();

			var player = await _context.Players.FindAsync(playerId);
			if (player == null || player.GuildId != guildId)
			{
				answer.Error = "Wrong data";
				return answer;
			}


			var guild = await _context.GuildDatas.FindAsync(guildId);
			if (guild == null || guild.LeaderId != playerId || guild.CurrentPlayerCount == guild.MaxPlayerCount)
			{
				answer.Error = "Wrong data";
				return answer;
			}

			var request = await _context.GuildPlayerRequests.FindAsync(requestId);
			if (request == null)
			{
				answer.Error = "Wrong data";
				return answer;
			}

			var recruit = await _context.Players.FindAsync(request.PlayerId);
			if (recruit == null || recruit.GuildId > 0)
			{
				answer.Error = "Wrong data";
				return answer;
			}

			recruit.GuildId = guildId;
			guild.CurrentPlayerCount += 1;

			var requestPlayer = await _context.Players.FindAsync(request.PlayerId);
			if (requestPlayer != null)
			{
				requestPlayer.GuildId = request.GuildId;
				var recruitPlayer = await _context.RecruitDatas.FindAsync(request.PlayerId);
				if (recruitPlayer != null)
				{
					recruitPlayer.GuildId = request.GuildId;
				}
			}

			var requests = await _context.GuildPlayerRequests.ToListAsync();
			var playerRequests = requests.FindAll(request => request.PlayerId == recruit.Id);

			_context.GuildPlayerRequests.RemoveRange(playerRequests);
			await _context.SaveChangesAsync();

			var allRecruits = await _context.RecruitDatas.ToListAsync();
			var guildRecruits = allRecruits.FindAll(recruit => recruit.GuildId == player.GuildId);

			var newTasks = await RefreshTasks(new List<GameTask>(), guild, playerId);
			var guildPlayerSaveContainer = new GuildPlayerSaveContainer(guild, guildRecruits, newTasks);
			answer.Result = _jsonConverter.Serialize(guildPlayerSaveContainer);
			return answer;
		}

		[HttpPost]
		[Route("Guild/DenyRequest")]
		public async Task<AnswerModel> DenyRequest(int playerId, int guildId, int requestId)
		{
			var answer = new AnswerModel();

			var player = await _context.Players.FindAsync(playerId);
			if (player == null || player.GuildId != guildId)
			{
				answer.Error = "Wrong data";
				return answer;
			}

			var guild = await _context.GuildDatas.FindAsync(guildId);
			if (guild == null || guild.LeaderId != playerId)
			{
				answer.Error = "Wrong data";
				return answer;
			}

			var request = await _context.GuildPlayerRequests.FindAsync(requestId);
			if (request == null || request.GuildId != player.GuildId)
			{
				answer.Error = "Wrong data";
				return answer;
			}

			_context.GuildPlayerRequests.Remove(request);
			await _context.SaveChangesAsync();

			answer.Result = "Success";
			return answer;
		}

		[HttpGet]
		[Route("Guild/TestModel")]
		public async Task<AnswerModel> TestModel([FromQuery] TestSendModel testSendModel)
		{
			var answer = new AnswerModel();
			Console.WriteLine(testSendModel.PlayerId);
			Console.WriteLine(testSendModel.Name);
			answer.Result = "Success";
			return answer;
		}

		[HttpPost]
		[Route("Guild/CreatePlayerBan")]
		public async Task<AnswerModel> CreatePlayerBan(int playerId, int guildId, int otherPlayerId, int reason)
		{
			var answer = new AnswerModel();

			var player = await _context.Players.FindAsync(playerId);
			if (player == null || player.GuildId != guildId)
			{
				answer.Error = "Wrong data";
				return answer;
			}

			var guild = await _context.GuildDatas.FindAsync(guildId);
			if (guild == null || guild.LeaderId != playerId)
			{
				answer.Error = "Wrong data";
				return answer;
			}

			var requests = await _context.GuildPlayerRequests.ToListAsync();
			var suitableRequest = requests.Find(request => request.GuildId == guildId && request.PlayerId == otherPlayerId);
			if (suitableRequest != null)
				_context.GuildPlayerRequests.Remove(suitableRequest);

			var playerBan = new GuildPlayerBan
			{
				PlayerId = playerId,
				GuildId = guildId,
				Reason = reason
			};

			await _context.GuildPlayerBans.AddAsync(playerBan);
			await _context.SaveChangesAsync();

			answer.Result = "Success";
			return answer;
		}

		[HttpPost]
		[Route("Guild/RemovePlayerBan")]
		public async Task<AnswerModel> RemovePlayerBan(int playerId, int guildId, int banId)
		{
			var answer = new AnswerModel();

			var player = await _context.Players.FindAsync(playerId);
			if (player == null || player.GuildId != guildId)
			{
				answer.Error = "Wrong data";
				return answer;
			}

			var guild = await _context.GuildDatas.FindAsync(guildId);
			if (guild == null || guild.LeaderId != playerId)
			{
				answer.Error = "Wrong data";
				return answer;
			}

			var ban = await _context.GuildPlayerBans.FindAsync(banId);
			if (ban == null)
			{
				answer.Error = "Wrong data";
				return answer;
			}

			_context.GuildPlayerBans.Remove(ban);
			await _context.SaveChangesAsync();

			answer.Result = "Success";
			return answer;
		}

		[HttpPost]
		[Route("Guild/LeaveFromGuild")]
		public async Task<AnswerModel> LeaveFromGuild(int playerId, int guildId)
		{
			var answer = new AnswerModel();

			var player = await _context.Players.FindAsync(playerId);
			if (player == null || player.GuildId != guildId)
			{
				answer.Error = "Wrong data";
				return answer;
			}

			var guild = await _context.GuildDatas.FindAsync(guildId);
			if (guild == null)
			{
				answer.Error = "Wrong data";
				return answer;
			}

			player.GuildId = 0;
			guild.CurrentPlayerCount -= 1;

			if (guild.LeaderId == playerId && guild.CurrentPlayerCount > 0)
			{
				var players = await _context.Players.ToListAsync();
				var playersFromGuild = players.FindAll(player => player.GuildId == guildId);
				var random = new Random();
				var index = random.Next(playersFromGuild.Count);

				guild.LeaderId = playersFromGuild[index].Id;
			}

			if (guild.CurrentPlayerCount == 0)
				_context.GuildDatas.Remove(guild);

			await _context.SaveChangesAsync();

			answer.Result = "Success";
			return answer;
		}

		[HttpPost]
		[Route("Guild/KickOffPlayer")]
		public async Task<AnswerModel> KickOffPlayer(int playerId, int guildId, int otherPlayerId)
		{
			var answer = new AnswerModel();

			var player = await _context.Players.FindAsync(playerId);
			if (player == null || player.GuildId != guildId || playerId == otherPlayerId)
			{
				answer.Error = "Wrong data";
				return answer;
			}

			var guild = await _context.GuildDatas.FindAsync(guildId);
			if (guild == null || guild.LeaderId != playerId)
			{
				answer.Error = "Wrong data";
				return answer;
			}

			var otherPlayer = await _context.Players.FindAsync(otherPlayerId);
			if (otherPlayer == null || otherPlayer.GuildId != guildId)
			{
				answer.Error = "Wrong data";
				return answer;
			}

			player.GuildId = 0;
			guild.CurrentPlayerCount -= 1;

			await _context.SaveChangesAsync();

			answer.Result = "Success";
			return answer;
		}

		[HttpPost]
		[Route("Guild/NominateNewLeader")]
		public async Task<AnswerModel> NominateLeader(int playerId, int guildId, int newLeaderPlayerId)
		{
			var answer = new AnswerModel();

			var player = await _context.Players.FindAsync(playerId);
			if (player == null || player.GuildId != guildId)
			{
				answer.Error = "Wrong data";
				return answer;
			}

			var guild = await _context.GuildDatas.FindAsync(guildId);
			if (guild == null || guild.LeaderId != playerId)
			{
				answer.Error = "Wrong data";
				return answer;
			}

			var newLeader = await _context.Players.FindAsync(newLeaderPlayerId);
			if (newLeader == null || newLeader.GuildId != guildId)
			{
				answer.Error = "Wrong data";
				return answer;
			}

			guild.LeaderId = newLeaderPlayerId;

			await _context.SaveChangesAsync();

			answer.Result = "Success";
			return answer;
		}

		[HttpPost]
		[Route("Guild/DonateForEvolve")]
		public async Task<AnswerModel> DonateForEvolve(int playerId, int guildId, string donate)
		{
			var answer = new AnswerModel();

			var player = await _context.Players.FindAsync(playerId);
			if (player == null || player.GuildId != guildId)
			{
				answer.Error = "Wrong data";
				return answer;
			}

			var guild = await _context.GuildDatas.FindAsync(guildId);
			if (guild == null)
			{
				answer.Error = "Wrong data";
				return answer;
			}

			var bigDigit = _jsonConverter.Deserialize<BigDigit>(donate);
			var cost = new Resource { PlayerId = playerId, Type = ResourceType.Gold, Count = bigDigit.Mantissa, E10 = bigDigit.E10 };
			var permission = await _resourcesController.CheckResource(playerId, cost, answer);
			if (!permission)
				return answer;

			await _resourcesController.SubstactResources(cost);

			var storage = new GameResource(ResourceType.Gold, guild.StorageMantissa, guild.StorageE10);
			storage.AddResource(bigDigit.Mantissa, bigDigit.E10);

			var targetDonate = _commonDictionaries.CostContainers[DONATE_EVOLUTION_COST_CONTAINER_KEY]
				.GetCostForLevelUp(guild.DonateEvoleLevel, playerId);

			var costLevelUp = new BigDigit(targetDonate[0].Count, targetDonate[0].E10);
			if (storage.Amount.CheckCount(costLevelUp))
			{
				storage.SubtractResource(costLevelUp.Mantissa, costLevelUp.E10);
				guild.DonateEvoleLevel += 1;
			}

			guild.StorageMantissa = storage.Amount.Mantissa;
			guild.StorageE10 = storage.Amount.E10;

			var recruit = await _context.RecruitDatas.FindAsync(playerId);
			if (recruit != null && !recruit.TodayDonate)
			{
				recruit.TodayDonate = true;
				AddExpirience(guild, 20);
			}

			await _context.SaveChangesAsync();

			answer.Result = _jsonConverter.Serialize(guild);
			return answer;
		}

		[HttpPost]
		[Route("Guild/RaidBoss")]
		public async Task<AnswerModel> RaidBoss(int playerId)
		{
			var answer = new AnswerModel();

			var player = await _context.Players.FindAsync(playerId);
			if (player == null || player.GuildId == 0)
			{
				answer.Error = "Player Wrong data";
				return answer;
			}

			var guild = await _context.GuildDatas.FindAsync(player.GuildId);
			if (guild == null)
			{
				answer.Error = "Guild wrong data";
				return answer;
			}
			var recruit = await _context.RecruitDatas.FindAsync(playerId);

			if (recruit == null || recruit.PlayerId != playerId)
			{
				GameLogging.WriteGameLog($"recruit not found or recruit.PlayerId != playerId, recruit null: {recruit == null}, playerId: {playerId}");
				var allRecruit = await _context.RecruitDatas.ToListAsync();
				recruit = allRecruit.Find(recruit => recruit.PlayerId == playerId);
			}

			if (recruit == null)
			{
				answer.Error = "Recruit not found!";
				return answer;
			}

			await CheckRefreshRecruitRaids(recruit);

			if (recruit.CountRaidBoss >= BOSS_FREE_RAID_COUNT)
			{
				var cost = GetCostRaid(recruit);

				var enoughResource = await _resourcesController.CheckResource(playerId, cost, answer);
				if (!enoughResource)
				{
					return answer;
				}

				await _resourcesController.SubstactResources(cost);
			}

			recruit.CountRaidBoss += 1;

			var damage = new BigDigit(1, 3);
			var currentHelth = new BigDigit(guild.BossHealthMantissa, guild.BossHealthE10);
			currentHelth.Subtract(damage);

			var currentRecruitDamage = new BigDigit(recruit.ResultMantissa, recruit.ResultE10);
			currentRecruitDamage.Add(damage);
			recruit.ResultMantissa = currentRecruitDamage.Mantissa;
			recruit.ResultE10 = currentRecruitDamage.E10;

			if (currentHelth.EqualsZero() || currentHelth.Mantissa < 0f)
			{
				guild.CurrentBoss += 1;
				var container = _commonDictionaries.GuildBossContainers["MainBosses"];
				var bossData = container.Missions[guild.CurrentBoss].BossModels[0];
				currentHelth = new BigDigit(bossData.Health.Mantissa, bossData.Health.E10);
			}

			guild.BossHealthMantissa = currentHelth.Mantissa;
			guild.BossHealthE10 = currentHelth.E10;

			if (!recruit.TodayRaidBoss)
			{
				recruit.TodayRaidBoss = true;
				AddExpirience(guild, 30);
			}

			await _context.SaveChangesAsync();

			var allRecruits = await _context.RecruitDatas.ToListAsync();
			var guildRecruits = allRecruits.FindAll(recruit => recruit.GuildId == player.GuildId);


			List<GameTask> playerTasks = new();

			var allTasks = await _context.GameTasks.ToListAsync();


			var playerAllTasks = allTasks.FindAll(task => task.PlayerId == playerId);
			foreach (var task in playerAllTasks)
			{
				if (_commonDictionaries.GameTaskModels.TryGetValue(task.TaskModelId, out var taskModel))
				{
					if (taskModel.SourceType == GameTaskSourceType.Guild)
					{
						playerTasks.Add(task);
					}

				}
			}

			var guildPlayerSaveContainer = new GuildPlayerSaveContainer(guild, guildRecruits, playerTasks);
			answer.Result = _jsonConverter.Serialize(guildPlayerSaveContainer);
			return answer;
		}

		[HttpPost]
		[Route("Guild/PlayerEnter")]
		public async Task<AnswerModel> PlayerEnter(int playerId)
		{
			var answer = new AnswerModel();

			var player = await _context.Players.FindAsync(playerId);
			if (player == null)
			{
				answer.Error = "Wrong data";
				return answer;
			}

			if (player.GuildId <= 0)
			{
				answer.Error = "Wrong message";
				return answer;
			}

			var guild = await _context.GuildDatas.FindAsync(player.GuildId);
			if (guild == null)
			{
				answer.Error = "Wrong data";
				return answer;
			}

			var recruit = await _context.RecruitDatas.FindAsync(playerId);
			if (recruit == null)
			{
				answer.Error = "Not found recruit";
				return answer;
			}

			if (recruit.TodayEnter)
			{
				answer.Error = "Today not work";
				return answer;
			}

			AddExpirience(guild, 10);
			recruit.TodayEnter = true;
			await _context.SaveChangesAsync();

			answer.Result = $"{guild.Expirience}";
			return answer;
		}

		private void AddExpirience(GuildData guild, int value)
		{
			guild.Expirience += value;
		}

		private async Task CheckRefreshRecruitRaids(RecruitData recruit)
		{
			var now = DateTime.UtcNow;
			if (!string.IsNullOrEmpty(recruit.DateTimeFirstRaidBoss))
			{
				var dateTime = DateTimeUtils.TryParseOrNow(recruit.DateTimeFirstRaidBoss);
				var delta = now - dateTime;
				if (delta.TotalHours >= BOSS_RAID_REFRESH_HOURS)
				{
					recruit.CountRaidBoss = 0;
					await _context.SaveChangesAsync();
				}
			}
		}

		private Resource GetCostRaid(RecruitData recruit)
		{
			var count = recruit.CountRaidBoss - BOSS_FREE_RAID_COUNT + 1;
			var gameResource = new GameResource(ResourceType.Diamond, new BigDigit(count * BOSS_RAID_COST_STEP));
			var result = new Resource(recruit.PlayerId, gameResource);
			return result;
		}

		private void GetNewTasks(int playerId, int count, GameTaskSourceType gameTaskSourceType, List<GuildTaskPosibilityContainer> taskPosibilityContainers, out List<GameTask> resultTasks)
		{
			var gameTaskModels = _commonDictionaries.GameTaskModels
				.Where(task => task.Value.SourceType == gameTaskSourceType)
				.Select(task => task.Value)
				.ToList();

			resultTasks = new List<GameTask>(count);

			if (taskPosibilityContainers.Count == 0)
				return;

			if (gameTaskModels.Count == 0)
				return;


			float sum = 0;
			foreach (var taskProbability in taskPosibilityContainers)
			{
				sum += taskProbability.Posibility;
			}

			for (var i = 0; i < count; i++)
			{
				float probabilityRandom = (float)_random.NextDouble() * sum;

				var index = -1;
				var currentSum = probabilityRandom;
				for (var j = 0; j < taskPosibilityContainers.Count; j++)
				{
					currentSum -= taskPosibilityContainers[j].Posibility;
					index += 1;
					if (currentSum < 0f)
						break;
				}

				index = Math.Clamp(index, 0, taskPosibilityContainers.Count);

				var selectRating = taskPosibilityContainers[index].Rating;

				var ratingTaskModels = gameTaskModels
					.Where(task => task.Rating == selectRating)
					.ToList();

				if(ratingTaskModels.Count == 0)
				{
					ratingTaskModels = gameTaskModels;
				}

				var random = _random.Next(0, ratingTaskModels.Count);
				var taskModel = ratingTaskModels[random];

				var intFactorDelta = (int)(taskModel.FactorDelta * 100f);
				var randFactor = _random.Next(100 - intFactorDelta, 100 + intFactorDelta + 1) / 100f;
				randFactor = (float)Math.Round(randFactor, 2);

				var newTask = new GameTask(playerId, taskModel, randFactor);
				resultTasks.Add(newTask);
			}
		}
	}
}
