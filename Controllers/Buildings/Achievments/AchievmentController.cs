using Microsoft.AspNetCore.Mvc;
using UniverseRift.Contexts;
using UniverseRift.GameModelDatas.Cities;

namespace UniverseRift.Controllers.Buildings.Achievments
{
    public class AchievmentController : ControllerBase, IAchievmentController
    {
        private readonly AplicationContext _context;

        public AchievmentController(AplicationContext context)
        {
            _context = context;
        }

        public async Task<AchievmentStorageData> GetPlayerSave(int playerId)
        {
            var result = new AchievmentStorageData();
            return result;
        }
    }
}
