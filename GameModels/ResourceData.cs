using UniverseRift.GameModels.Common;
using UniverseRift.Models.Resources;

namespace Models.Data.Inventories
{
    public class ResourceData : InventoryBaseItem
    {
        public ResourceType Type;
        public BigDigit Amount = new BigDigit();
    }
}