using System.ComponentModel.DataAnnotations;

namespace UniverseRift.Models.Players
{
    public class PlayerProgress
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int CampaignProgress { get; set; } = -1;
        public int ChellangeTowerProgress { get; set; }
        public string LastGetAutoFightReward { get; set; } = string.Empty;
    }
}
