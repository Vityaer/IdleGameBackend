using Microsoft.AspNetCore.Mvc;
using UniverseRift.Contexts;
using UniverseRift.Models.DailyTasks;

namespace UniverseRift.Controllers.Buildings.DailyTasks
{
    public class DailyTasksController : ControllerBase, IDailyTasksController
    {
        private readonly AplicationContext _context;

        public DailyTasksController(AplicationContext context)
        {
            _context = context;
        }

        public async Task<DailyTaskContainer> GetPlayerSave(int playerId)
        {
            var result = new DailyTaskContainer();
            return result;
        }
    }
}
