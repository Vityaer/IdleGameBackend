using Microsoft.AspNetCore.Mvc;
using UniverseRift.Contexts;
using UniverseRift.GameModelDatas.Cities;

namespace UniverseRift.Controllers.Buildings.Tutorials
{
    public class TutorialController : ControllerBase, ITutorialController
    {
        private readonly AplicationContext _context;

        public TutorialController(AplicationContext context)
        {
            _context = context;
        }

        public async Task<SimpleBuildingData> GetPlayerSave(int playerId)
        {
            var result = new SimpleBuildingData();
            return result;
        }
    }
}
