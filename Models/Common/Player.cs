namespace UniverseRift.Models.Common
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ServerId { get; set; } = 0;
        public int Level { get; set; } = 1;
        public int GuildId { get; set; } = 0;
        public int VipLevel { get; set; } = 0;
        public string LastEnteredDateTime { get; set; } = string.Empty;
        public string LastUpdateGameData { get; set; } = string.Empty;
    }
}
