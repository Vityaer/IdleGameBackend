namespace UniverseRift.Models.Misc
{
    public class FriendshipRequest
    {
        public int Id { get; set; }
        public int SenderPlayerId { get; set; }
        public int TargetPlayerId { get; set; }
        public string Date { get; set; } = string.Empty;
    }
}
