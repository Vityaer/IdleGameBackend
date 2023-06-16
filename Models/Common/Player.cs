namespace UniverseRift.Models.Common
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ServerId { get; set; }
        public int Level { get; set; }
        public int GuildId { get; set; }
        public int VipLevel { get; set; }
    }
}
