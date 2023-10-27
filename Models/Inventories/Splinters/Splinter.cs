using UniverseRift.Models.Common;

namespace UniverseRift.Models.Inventories.Splinters
{
    public class Splinter : BaseInventoryObject
    {
        public Splinter() { }

        public Splinter(int playerId, string splinterId, int amount)
        {
            PlayerId = playerId;
            SplinterId = splinterId;
            Count = amount;
        }

        public string SplinterId { get; set; } = string.Empty;
    }
}
