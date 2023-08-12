using Microsoft.AspNetCore.Mvc;
using UniverseRift.Contexts;
using UniverseRift.GameModelDatas.Cities;

namespace UniverseRift.Controllers.Buildings.Arenas
{
    public class ArenaController : ControllerBase, IArenaController
    {
        private readonly AplicationContext _context;

        public ArenaController(AplicationContext context)
        {
            _context = context;
        }

        public async Task<ArenaBuildingModel> GetPlayerSave(int playerId)
        {
            var result = new ArenaBuildingModel();
            return result;
        }
    }
}
