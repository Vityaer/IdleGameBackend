using UniverseRift.GameModelDatas.Players;
using UniverseRift.Models.Events;

namespace UniverseRift.Controllers.Buildings.GameCycles
{
    public interface IGameCycleController
    {
        Task<CycleEventsData> GetPlayerSave(int playerId);
        void SetChangeCycle(GameEventType newEventType);
        void OnChangeCycle(GameEventType oldEventType, GameEventType newEventType);
    }
}
