using Microsoft.AspNetCore.Mvc;
using Misc.Json;
using Models;
using Models.Data.Inventories;
using UniverseRift.Contexts;
using UniverseRift.Controllers.Common;
using UniverseRift.GameModelDatas.Players;
using UniverseRift.Heplers.GameLogging;
using UniverseRift.Models.Common.Server;
using UniverseRift.Models.Events;
using UniverseRift.Models.Resources;

namespace UniverseRift.Controllers.Buildings.GameCycles
{
    public class GameCycleController : ControllerBase, IGameCycleController
    {
        private const int CANDY_COUNT = 6;

        private readonly AplicationContext _context;
        private readonly ICommonDictionaries _commonDictionaries;
        private readonly IJsonConverter _jsonConverter;


		public GameCycleController(AplicationContext context, ICommonDictionaries commonDictionaries, IJsonConverter jsonConverter)
        {
            _context = context;
            _commonDictionaries = commonDictionaries;
			_jsonConverter = jsonConverter;
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
			result.CurrentCycle = serverData.EventDataJSON;
			result.LastGetAlchemyDateTime = player.LastGetAlchemyDateTime;
            return result;
        }

        public void OnChangeCycle(ServerLifeTime server, GameEventType oldEventType, GameEventType newEventType)
        {
            TierDown(server, oldEventType);
            TierUp(server, newEventType);
        }

        public void SetChangeCycle(ServerLifeTime server, GameEventType newEventType)
        {
            TierUp(server, newEventType);
        }

        private void TierDown(ServerLifeTime server, GameEventType oldEventType)
        {

			switch (oldEventType)
            {
                case GameEventType.Sweet:
                    var achievmentContainer = _commonDictionaries.AchievmentContainers["DailyTasks"];
					if (!Enum.TryParse($"Candy{server.SweetEventNumber}", out ResourceType candyType))
						break;

					foreach (var id in achievmentContainer.TaskIds)
                    {
                        var achievment = _commonDictionaries.Achievments[id];

						foreach (var stage in achievment.Stages)
                        {
                            var resourceForRemove = stage.Reward.Resources.Find(res => res.Type == candyType);
                            if (resourceForRemove != null)
                                stage.Reward.Resources.Remove(resourceForRemove);
                        }
                    }

					break;
            }
		}

		private void TierUp(ServerLifeTime server, GameEventType newEventType)
        {
			AbstractCycleData eventData = null;

			switch (newEventType)
            {
                case GameEventType.Sweet:
                    var achievmentContainer = _commonDictionaries.AchievmentContainers["DailyTasks"];

					if (!Enum.TryParse($"Candy{server.SweetEventNumber}", out ResourceType candyType))
						break;

					foreach (var id in achievmentContainer.TaskIds)
                    {
                        var achievment = _commonDictionaries.Achievments[id];
                        foreach (var stage in achievment.Stages)
                        {
                            stage.Reward.Resources.Add(new ResourceData
                            {
                                Type = candyType,
                                Amount = new(CANDY_COUNT)
                            });
                        }
                    }

                    var sweetEventData = new SweetEventData();
                    sweetEventData.ResourceType = candyType;
                    eventData = sweetEventData;

                    var sweetMarket = _commonDictionaries.Markets["SweetCycleMarket"];

                    foreach (var sweetGoodId in sweetMarket.Products)
                    {
                        _commonDictionaries.Products[sweetGoodId].Cost.Type = candyType;
                    }
					break;
            }

			server.EventDataJSON = _jsonConverter.Serialize(eventData);
		}
	}
}
