using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Misc.Json;
using UniverseRift.Contexts;
using UniverseRift.Controllers.Common;
using UniverseRift.GameModels;
using UniverseRift.Models.City.FortuneWheels;
using UniverseRift.Models.FortuneWheels;
using UniverseRift.Models.Resources;
using UniverseRift.Models.Results;
using UniverseRift.Models.Rewards;
using UniverseRift.Services.Resources;
using UniverseRift.Services.Rewarders;

namespace UniverseRift.Controllers.Buildings.FortuneWheels
{
    public class FortuneWheelController : ControllerBase, IFortuneWheelController
    {
        private const int REWARD_COUNT = 8;

        private readonly ICommonDictionaries _commonDictionaries;
        private readonly AplicationContext _context;
        private readonly IJsonConverter _jsonConverter;
        private readonly IRewardService _clientRewardService;
        private readonly IResourceManager _resourceController;

        private readonly Random _random = new Random();

        public FortuneWheelController(
            AplicationContext context,
            IJsonConverter jsonConverter,
            ICommonDictionaries commonDictionaries,
            IRewardService clientRewardService,
            IResourceManager resourceController
            )
        {
            _resourceController = resourceController;
            _clientRewardService = clientRewardService;
            _commonDictionaries = commonDictionaries;
            _jsonConverter = jsonConverter;
            _context = context;
        }

        [HttpPost]
        [Route("FortuneWheel/Rotate")]
        public async Task<AnswerModel> Rotate(int playerId, int count)
        {
            var answer = new AnswerModel();
            if (count < 1)
            {
                answer.Error = $"data error count: {count}";
                return answer;
            }

            var cost = new Resource() { PlayerId = playerId, Type = ResourceType.CoinFortune, Count = count, E10 = 0 };
            var permission = await _resourceController.CheckResource(playerId, cost, answer);
            if (!permission)
            {
                return answer;
            }

            var playerWheel = await GetWheelModel(playerId);

            var posibleReward = _jsonConverter.Deserialize<FortuneWheelData>(playerWheel.RewardsJson);
            var reward = new RewardModel();

            for(var i = 0; i < count; i++)
            {
                var rand = _random.Next(0, posibleReward.Rewards.Count);
                var id = posibleReward.Rewards.ElementAt(rand).RewardModelId;
                var rewardModel = _commonDictionaries.FortuneRewardModels[id];
                reward.Add(rewardModel.Subject);
            }

            await _resourceController.SubstactResources(cost);
            await _clientRewardService.AddReward(playerId, reward);
            answer.Result = _jsonConverter.Serialize(reward);
            return answer;
        }

        [HttpPost]
        [Route("FortuneWheel/RefreshWheel")]
        public async Task<AnswerModel> RefreshWheel(int playerId)
        {
            var answer = new AnswerModel();

            var newWheel = CreateFortuneWheel(playerId);
            await _context.SaveChangesAsync();

            answer.Result = _jsonConverter.Serialize(newWheel);
            return answer;
        }

        public async Task<FortuneWheelModel> GetWheelModel(int playerId)
        {
            var playerWheel = await LoadFortuneWheel(playerId);

            if (playerWheel == null)
            {
                playerWheel = await CreateFortuneWheel(playerId);
            }

            return playerWheel;
        }

        private async Task<FortuneWheelModel> CreateFortuneWheel(int playerId)
        {
            var playerWheel = await LoadFortuneWheel(playerId);
            var newWheel = new FortuneWheelModel();

            var rewardsData = new FortuneWheelData();

            var rewardModels = _commonDictionaries.FortuneRewardModels.ToList();
            for (var i = 0; i < REWARD_COUNT; i++)
            {
                var rand = _random.Next(0, rewardModels.Count);
                var randomReward = rewardModels[rand].Value;
                rewardsData.Rewards.Add(new FortuneRewardData { RewardModelId = randomReward.Id });
            }
            newWheel.RewardsJson = _jsonConverter.Serialize(rewardsData);

            if (playerWheel == null)
            {
                await _context.FortuneWheels.AddAsync(newWheel);
            }

            playerWheel = newWheel;
            await _context.SaveChangesAsync();

            return playerWheel;
        }

        private async Task<FortuneWheelModel> LoadFortuneWheel(int playerId)
        {
            var wheels = await _context.FortuneWheels.ToListAsync();
            var playerWheel = wheels.Find(wheel => wheel.PlayerId == playerId);
            return playerWheel;
        }

        public async Task<FortuneWheelData> GetPlayerSave(int playerId)
        {
            var playerWheel = await GetWheelModel(playerId);

            var result = _jsonConverter.Deserialize<FortuneWheelData>(playerWheel.RewardsJson);

            return result;
        }
    }
}
