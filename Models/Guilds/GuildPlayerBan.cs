namespace UniverseRift.Models.Guilds
{
    public class GuildPlayerBan
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int GuildId { get; set; }
        public int Reason { get; set; }
    }
}
