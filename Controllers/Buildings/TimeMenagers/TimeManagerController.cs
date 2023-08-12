using Microsoft.AspNetCore.Mvc;
using UniverseRift.Contexts;
using UniverseRift.GameModelDatas.Cities;

namespace UniverseRift.Controllers.Buildings.TimeMenagers
{
    public class TimeManagerController : ControllerBase, ITimeManagerController
    {
        private readonly AplicationContext _context;

        public TimeManagerController(AplicationContext context)
        {
            _context = context;
        }

        public async Task<TimeManagementData> GetPlayerSave(int playerId)
        {
            var result = new TimeManagementData();
            return result;
        }
    }
}
