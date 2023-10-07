using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniverseRift.Contexts;
using UniverseRift.Models.City.DailyRewards;

namespace UniverseRift.Controllers.Buildings.DailyRewards
{
    public class DailyRewardController : ControllerBase, IDailyRewardController
    {
        private readonly AplicationContext _context;

        public DailyRewardController(AplicationContext context)
        {
            _context = context;
        }

        public async Task<DailyRewardContainer> GetPlayerSave(int playerId, bool flagCreateNewData)
        {
            var dailyRewardSaves = await _context.DailyRewardProgresses.ToListAsync();
            var playerData = dailyRewardSaves.Find(save => save.PlayerId == playerId);

            if (playerData == null)
            {
                playerData = new DailyRewardProgress();
                playerData.PlayerId = playerId;
                //playerData.RewardId = CreateRewards();
            }
            var result = new DailyRewardContainer();

            return result;
        }

        //private string CreateRewards()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
