using UniverseRift.GameModels;

namespace UniverseRift.Models.Tasks.SimpleTask
{
    public class GameTaskModel : BaseModel
    {
        public int Rating;
        public int RequireHour;
        public float FactorDelta;
        public RewardModel Reward;
		public GameTaskSourceType SourceType;
	}
}
