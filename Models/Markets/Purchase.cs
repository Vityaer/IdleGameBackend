namespace UniverseRift.Models.Markets
{
    public class Purchase
    {
        public int Id { get; set; }
        public string ProductId { get; set; } = string.Empty;
        public int PlayerId { get; set; }
        public int PurchaseCount { get; set; }
    }
}
