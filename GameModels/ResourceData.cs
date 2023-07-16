using UniverseRift.Models.Resources;

namespace UniverseRift.GameModels
{
    public class ResourceData : InventoryBaseItem
    {
        public ResourceType Type;
        public BigDigit Amount = new BigDigit();
    }
}