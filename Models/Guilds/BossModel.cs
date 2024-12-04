using UniverseRift.GameModels.Common;

namespace UniverseRift.Models.Guilds
{
    public class BossModel
    {
        public string HeroId;

        public int Level = 1;
        public int Rating = 1;

        public BigDigit Attack;
        public BigDigit Health;
    }
}
