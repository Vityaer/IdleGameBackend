namespace UniverseRift.Models.Common
{
    public abstract class BaseInventoryObject
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public float Count { get; set; }
        public int E10 { get; set; }
    }
}
