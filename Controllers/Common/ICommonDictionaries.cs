using UniverseRift.GameModels;

namespace UniverseRift.Controllers.Common
{
    public interface ICommonDictionaries
    {
        public Dictionary<string, CampaignChapterModel> CampaignChapters { get; }
        public Dictionary<string, CostLevelUpContainer> HeroesCostLevelUps { get; }
        public Dictionary<string, RewardModel> Rewards { get; }
    }
}
