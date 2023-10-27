using UniverseRift.Controllers.Common;
using UniverseRift.MessageData;

namespace UniverseRift.GameModels
{
    public class MissionModel
    {
        public string Name;
        public string Location;
        public List<HeroData> Units = new List<HeroData>();
        public RewardModel WinReward;
    }
}