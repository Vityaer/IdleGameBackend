using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UniverseRift.Models.Inventories.Resources;

namespace UniverseRift.Models.Markets
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; } = string.Empty;
        public int PurchaseCount { get; }

        public ResourceType CostType;
        public float Cost { get; set; }
        public int E10 { get; set; }
        public RecoveryType RecoveryType { get; set; }
    }
}
