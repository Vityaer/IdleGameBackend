using UniverseRift.GameModelDatas.Players;
using UniverseRift.Models.Common.Server;
using UniverseRift.Models.Events;

namespace UniverseRift.Controllers.Buildings.GameCycles
{
    public interface IGameCycleController
    {
        Task<CycleEventsData> GetPlayerSave(int playerId);
        void SetChangeCycle(ServerLifeTime server, GameEventType newEventType);
        void OnChangeCycle(ServerLifeTime server, GameEventType oldEventType, GameEventType newEventType);
    }
}
