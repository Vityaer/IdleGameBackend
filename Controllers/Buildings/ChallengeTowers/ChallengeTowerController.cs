using Microsoft.AspNetCore.Mvc;
using UniverseRift.Contexts;
using UniverseRift.GameModelDatas.Cities;

namespace UniverseRift.Controllers.Buildings.ChallengeTowers
{
    public class ChallengeTowerController : ControllerBase, IChallengeTowerController
    {
        private readonly AplicationContext _context;

        public ChallengeTowerController(AplicationContext context)
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
