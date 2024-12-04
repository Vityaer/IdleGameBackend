using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Misc.Json;
using Models.City.FortuneRewards;
using UniverseRift.Contexts;
using UniverseRift.Controllers.Buildings.Achievments;
using UniverseRift.Controllers.Common;
using UniverseRift.GameModels;
using UniverseRift.GameModels.FortuneWheels;
using UniverseRift.Models.Achievments;
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
        private readonly IAchievmentController _achievmentController;

        private readonly Random _random = new Random();

        public FortuneWheelController(
            AplicationContext context,
            IJsonConverter jsonConverter,
            ICommonDictionaries commonDictionaries,
            IRewardService clientRewardService,
            IResourceManager resourceController,
            IAchievmentController achievmentController
            )
        {
            _achievmentController = achievmentController;
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

            var rand = 0;
            for(var i = 0; i < count; i++)
            {
                rand = _random.Next(0, posibleReward.Rewards.Count);
                var id = posibleReward.Rewards.ElementAt(rand).RewardModelId;
                var rewardModel = _commonDictionaries.FortuneRewardModels[id];

                switch (rewardModel)
                {
                    case FortuneResourseRewardModel resourseRewardModel:
                        reward.Add(resourseRewardModel.Subject);
                        break;
                    case FortuneItemRewardModel itemRewardModel:
                        reward.Add(itemRewardModel.Subject);
                        break;
                }
            }

            await _resourceController.SubstactResources(cost);
            await _clientRewardService.AddReward(playerId, reward);

            await _achievmentController.AchievmentUpdataData(playerId, "FortuneSimpleSpinAchievment", count);

            var rewardContainer = new FortuneRewardContainer{ Reward = reward, ResultItemIndex = rand };
            answer.Result = _jsonConverter.Serialize(rewardContainer);
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
            newWheel.PlayerId = playerId;
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

        public async Task<FortuneWheelData> GetPlayerSave(int playerId, bool flagCreateNewData)
        {
            var playerWheel = await LoadFortuneWheel(playerId);

            if (playerWheel == null || flagCreateNewData)
            {
                playerWheel = await CreateFortuneWheel(playerId);
            }

            var result = _jsonConverter.Deserialize<FortuneWheelData>(playerWheel.RewardsJson);

            return result;
        }
    }
}
