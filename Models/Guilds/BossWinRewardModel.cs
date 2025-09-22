using UniverseRift.GameModels;

namespace Models.Data.Rewards
{
	public class BossWinRewardModel
	{
		public int StartIndex;
		public int EndIndex;
		public RewardModel Reward;

		public BossWinRewardModel(RewardModel reward)
		{
			Reward = reward;
		}

		public BossWinRewardModel() { }
	}
}
