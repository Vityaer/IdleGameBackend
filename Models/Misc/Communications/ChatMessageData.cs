namespace UniverseRift.Models.Misc.Communications
{
    public class ChatMessageData
    {
        public int Id { get; set; }
        public int PlayerWritterId { get; set; }
        public string Message { get; set; } = string.Empty;
        public string CreateDateTime { get; set; } = string.Empty;
    }
}
