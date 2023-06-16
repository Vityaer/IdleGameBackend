using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UniverseRift.Controllers.Players.Inventories.Items;

namespace UniverseRift.Models.Items
{
    public class ItemTemplate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Name { get; set; } = string.Empty;
        public ItemType Type { get; set; }
    }
}
