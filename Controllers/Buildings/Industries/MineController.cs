using Microsoft.AspNetCore.Mvc;
using UniverseRift.Contexts;
using UniverseRift.Models.Results;

namespace UniverseRift.Controllers.Buildings.Industries
{
    public class MineController : ControllerBase
    {
        private readonly AplicationContext _context;

        public MineController(AplicationContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("Mines/Create")]
        public async Task<AnswerModel> MineCreate(int playerId, string mineId)
        {
            var answer = new AnswerModel();

            answer.Result = "Success";
            return answer;
        }

        [HttpPost]
        [Route("Mines/LevelUp")]
        public async Task<AnswerModel> MineLevelUp(int playerId, int mineId)
        {
            var answer = new AnswerModel();

            answer.Result = "Success";
            return answer;
        }
    }
}
