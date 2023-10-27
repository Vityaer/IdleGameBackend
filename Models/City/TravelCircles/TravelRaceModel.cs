using UniverseRift.GameModels;

namespace UniverseRift.Models.City.TravelCircles
{
    public class TravelRaceModel : BaseModel
    {
        public string Race;
        public List<MissionWithSmashReward> Missions = new List<MissionWithSmashReward>();
    }
}
