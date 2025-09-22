using Models.Data.Rewards;

namespace UniverseRift.Models.Guilds
{
    public class GuildBossMission
    {
        public List<BossModel> BossModels = new();
        public List<BossWinRewardModel> RewardModels = new();
    }
}
