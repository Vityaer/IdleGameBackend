using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Data.Inventories;
using UniverseRift.Contexts;
using UniverseRift.Controllers.Common;
using UniverseRift.GameModelDatas.Players;
using UniverseRift.Heplers.GameLogging;
using UniverseRift.Models.Events;
using UniverseRift.Models.Resources;

namespace UniverseRift.Controllers.Buildings.GameCycles
{
    public class GameCycleController : ControllerBase, IGameCycleController
    {
        private const int CANDY_COUNT = 6;

        private readonly AplicationContext _context;
        private readonly ICommonDictionaries _commonDictionaries;

        public GameCycleController(AplicationContext context, ICommonDictionaries commonDictionaries)
        {
            _context = context;
            _commonDictionaries = commonDictionaries;
        }

        public async Task<CycleEventsData> GetPlayerSave(int playerId)
        {
            var result = new CycleEventsData();
            var serverData = await _context.ServerLifeTimes.FindAsync(1);
            if (serverData == null)
            {
                GameLogging.WriteGameLog($"Server data not found.");
                return result;
            }

            var player = await _context.Players.FindAsync(playerId);

            if (player == null)
            {
                return result;
            }

            result.CurrentEventType = serverData.EventType;
            result.StartGameCycleDateTime = serverData.NexGameCycle;
            result.LastGetAlchemyDateTime = player.LastGetAlchemyDateTime;
            return result;
        }

        public void OnChangeCycle(GameEventType oldEventType, GameEventType newEventType)
        {
            TierDown(oldEventType);
            TierUp(newEventType);
        }

        public void SetChangeCycle(GameEventType newEventType)
        {
            TierUp(newEventType);
        }

        private void TierDown(GameEventType oldEventType)
        {
            switch (oldEventType)
            {
                case GameEventType.Sweet:
                    var achievmentContainer = _commonDictionaries.AchievmentContainers["DailyTasks"];
                    foreach (var id in achievmentContainer.TaskIds)
                    {
                        var achievment = _commonDictionaries.Achievments[id];
                        foreach (var stage in achievment.Stages)
                        {
                            var resourceForRemove = stage.Reward.Resources.Find(res => res.Type == ResourceType.Candy);
                            if (resourceForRemove != null)
                                stage.Reward.Resources.Remove(resourceForRemove);
                        }
                    }
                    break;
            }
        }

        private void TierUp(GameEventType newEventType)
        {

            switch (newEventType)
            {
                case GameEventType.Sweet:
                    var achievmentContainer = _commonDictionaries.AchievmentContainers["DailyTasks"];
                    foreach (var id in achievmentContainer.TaskIds)
                    {
                        var achievment = _commonDictionaries.Achievments[id];
                        foreach (var stage in achievment.Stages)
                        {
                            stage.Reward.Resources.Add(new ResourceData
                            {
                                Type = ResourceType.Candy,
                                Amount = new(CANDY_COUNT)
                            });
                        }
                    }
                    break;
            }
        }
    }
}
