using UniverseRift.Controllers.Common;
using UniverseRift.GameModels;

namespace UniverseRift.Models.City.TravelCircles
{
    public class MissionWithSmashReward : MissionModel
    {
        protected RewardModel smashReward;
        public RewardModel SmashReward { get => smashReward; }

        public MissionWithSmashReward Clone()
        {
            return new MissionWithSmashReward
            {
                Name = this.Name,
                Units = this.Units,
                //WinReward = (GameReward)this.WinReward.Clone(),
                //smashReward = smashReward.Clone(),
                Location = this.Location
            };
        }
    }
}
