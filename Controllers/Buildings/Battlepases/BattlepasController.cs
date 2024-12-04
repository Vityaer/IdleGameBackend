using Microsoft.AspNetCore.Mvc;
using UniverseRift.Contexts;
using UniverseRift.Controllers.Common;
using UniverseRift.GameModels;
using UniverseRift.Models.Resources;
using UniverseRift.Models.Results;
using UniverseRift.Models.Rewards;
using UniverseRift.Services.Resources;

namespace UniverseRift.Controllers.Buildings.Battlepases
{
    public class BattlepasController : ControllerBase, IBattlepasController
    {
        private const string BATTLEPAS_NAME = "BattlepasRewards";

        private readonly AplicationContext _context;
        private readonly ICommonDictionaries _commonDictionaries;
        private readonly IResourceManager _resourceManager;

        public BattlepasController(
            AplicationContext context,
            ICommonDictionaries commonDictionaries,
            IResourceManager resourceManager
            )
        {
            _context = context;
            _commonDictionaries = commonDictionaries;
            _resourceManager = resourceManager;
        }

        [HttpPost]
        [Route("Battlepas/GetNextReward")]
        public async Task<AnswerModel> GetNextReward(int playerId)
        {
            var answer = new AnswerModel();
            var battlepas = await _context.BattlepasDatas.FindAsync(playerId);
            if (battlepas == null)
            {
                answer.Error = "Wrong data";
                return answer;
            }

            var nextIndex = battlepas.CurrentDailyBattlepasStage + 1;
            var rewards = _commonDictionaries.RewardContainerModels[BATTLEPAS_NAME].Rewards;
            if (nextIndex >= rewards.Count)
            {
                answer.Error = "Wrong data";
                return answer;
            }

            var requireCount = (nextIndex + 1) * 100;
            var gameResource = new GameResource(ResourceType.EventAgentMonet, requireCount);
            var resource = new Resource(playerId, gameResource);
            var resourceEnough = await _resourceManager.CheckResource(playerId, resource, answer);
            if (!resourceEnough)
            {
                answer.Error = "Wrong data";
                return answer;
            }

            battlepas.CurrentDailyBattlepasStage = nextIndex;
            await _context.SaveChangesAsync();
            answer.Result = "Success";
            return answer;
        }

        public async Task<BattlepasData> GetPlayerSave(int playerId, bool flagNewDay)
        {
            return await _context.BattlepasDatas.FindAsync(playerId);
        }

        public async Task OnRegisterPlayer(int playerId)
        {
            var battlepas = new BattlepasData(playerId);
            await _context.BattlepasDatas.AddAsync(battlepas);
            await _context.SaveChangesAsync();
        }


    }
}
