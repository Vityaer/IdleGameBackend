using Microsoft.AspNetCore.Mvc;
using UniverseRift.Contexts;
using UniverseRift.GameModelDatas.Cities;

namespace UniverseRift.Controllers.Buildings.TravelCircles
{
    public class TravelCircleController : ControllerBase, ITravelCircleController
    {
        private readonly AplicationContext _context;

        public TravelCircleController(AplicationContext context)
        {
            _context = context;
        }

        public async Task<BuildingWithFightTeamsData> GetPlayerSave(int playerId)
        {
            var result = new BuildingWithFightTeamsData();
            return result;
        }
    }
}
