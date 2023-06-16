using Microsoft.AspNetCore.Mvc;
using UniverseRift.Contexts;
using UniverseRift.Models.Missions;
using UniverseRift.Models.Players;

namespace UniverseRift.Controllers.Common.GameProgress
{
    public class PlayerProgressController : ControllerBase
    {
        private readonly AplicationContext _context;

        public PlayerProgressController(AplicationContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("Campaign/CompleteMission")]
        public async Task CompleteMission(int playerId, MissionType missionType)
        {
            var playerProgress = await _context.PlayerProgresses.FindAsync(playerId);

            if (playerProgress == null)
            {
                playerProgress = new PlayerProgress() { PlayerId = playerId, CampaignProgress = 0, ChellangeTowerProgress = 0};
                await _context.PlayerProgresses.AddAsync(playerProgress);
            }

            switch (missionType)
            {
                case MissionType.Campaign:
                    playerProgress.CampaignProgress += 1;
                    break;
                case MissionType.ChellageTower:
                    playerProgress.CampaignProgress += 1;
                    break;
            }

            await _context.SaveChangesAsync();
        }
    }
}
