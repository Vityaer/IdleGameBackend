using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UniverseRift.Misc;

namespace UniverseRift.Models.Heroes
{
    public class HeroTemplate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; } = string.Empty;
        public Race Race { get; set; }
        public Rare Rare { get; set; }
        public int Rating { get; set; }
        public string DefaultViewId { get; set; } = string.Empty;
    }
}
