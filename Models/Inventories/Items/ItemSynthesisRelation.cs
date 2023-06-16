namespace UniverseRift.Models.Items
{
    public class ItemSynthesisRelation
    {
        public int Id { get; set; }
        public string ResultItemName { get; set; } = string.Empty;
        public string ItemIngredientName { get; set; } = string.Empty;
        public int RequireCount { get; set; } = 3;

    }
}
