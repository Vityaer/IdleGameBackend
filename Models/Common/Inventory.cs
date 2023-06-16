using UniverseRift.Models.Inventories.Splinters;
using UniverseRift.Models.Items;
using UniverseRift.Models.Resources;

namespace UniverseRift.Models.Common
{
    public class Inventory
    {
        public int PlayerId { get; set; }
        public List<Resource> Resources { get; set; } = new List<Resource>();
        public List<Item> Items { get; set; } = new List<Item>();
        public List<Splinter> Splinters { get; set; } = new List<Splinter>();

        public Inventory()
        {
        }

        public Inventory(List<Resource> resources, List<Item> items, List<Splinter> splinters)
        {
            Resources = resources;
            Items = items;
            Splinters = splinters;
        }

    }
}
