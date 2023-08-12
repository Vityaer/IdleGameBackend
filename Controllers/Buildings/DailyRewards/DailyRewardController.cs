using Microsoft.AspNetCore.Mvc;
using UniverseRift.Contexts;
using UniverseRift.GameModelDatas.Cities;

namespace UniverseRift.Controllers.Buildings.DailyRewards
{
    public class DailyRewardController : ControllerBase, IDailyRewardController
    {
        private readonly AplicationContext _context;

        public DailyRewardController(AplicationContext context)
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
