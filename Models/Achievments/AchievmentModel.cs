using Models.Common.BigDigits;
using UniverseRift.GameModels;

namespace UniverseRift.Models.Achievments
{
    public class AchievmentModel : BaseModel
    {
        public AchievmentType Type;
        public ProgressType ProgressType;
        public List<AchievmentStageModel> Stages;
        public string ImplementationName;

        public BigDigit GetRequireFinishCount()
        {
            return Stages[Stages.Count - 1].RequireCount;
        }

        public BigDigit GetRequireCount(int stage)
        {
            return Stages[stage].RequireCount;
        }

        public RewardModel GetReward(int currentStage)
        {
            return Stages[currentStage].Reward;
        }
    }
}
