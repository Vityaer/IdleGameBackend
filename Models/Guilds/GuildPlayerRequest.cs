namespace UniverseRift.Models.Guilds
{
    public class GuildPlayerRequest
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int GuildId { get; set;}
        public string CreateDate { get; set; } = string.Empty;
    }
}
