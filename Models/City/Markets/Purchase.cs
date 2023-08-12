namespace UniverseRift.Models.City.Markets
{
    public class Purchase
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public string ProductId { get; set; } = string.Empty;
        public int PurchaseCount { get; set; }
    }
}
