using UniverseRift.Controllers.Players.Inventories.Resources;

namespace UniverseRift.Models.Rewards
{
    public class FortuneReward
    {
        public int Id { get; set; }
        public TypeObject Type { get; set; }
        public string Name { get; set; } = string.Empty;    
        public float Count { get; set; }
        public int E10 { get; set; }
        public float Posibility { get; set; }
    }
}
