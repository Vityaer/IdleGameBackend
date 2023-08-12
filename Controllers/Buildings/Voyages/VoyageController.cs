using Microsoft.AspNetCore.Mvc;
using UniverseRift.Contexts;
using UniverseRift.GameModelDatas.Cities;

namespace UniverseRift.Controllers.Buildings.Voyages
{
    public class VoyageController : ControllerBase, IVoyageController
    {
        private readonly AplicationContext _context;

        public VoyageController(AplicationContext context)
        {
            _context = context;
        }

        public async Task<VoyageBuildingData> GetPlayerSave(int playerId)
        {
            var result = new VoyageBuildingData();
            return result;
        }
    }
}
