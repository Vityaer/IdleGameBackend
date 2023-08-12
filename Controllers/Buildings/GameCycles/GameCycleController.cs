using Microsoft.AspNetCore.Mvc;
using UniverseRift.Contexts;
using UniverseRift.GameModelDatas.Players;

namespace UniverseRift.Controllers.Buildings.GameCycles
{
    public class GameCycleController : ControllerBase, IGameCycleController
    {
        private readonly AplicationContext _context;

        public GameCycleController(AplicationContext context)
        {
            _context = context;
        }

        public async Task<CycleEventsData> GetPlayerSave(int playerId)
        {
            var result = new CycleEventsData();
            return result;
        }
    }
}
