namespace UniverseRift.Models.Misc
{
    public class FriendshipData
    {
        public int Id { get; set; }
        public int FirstPlayerId { get; set; }
        public int SecondPlayerId { get; set; }
        public int Level { get; set; }
        public bool PresentForFirstPlayer { get; set; }
        public bool PresentForSecondPlayer { get; set; }
        public bool FirstPlayerRecieved { get; set; }
        public bool SecondPlayerRecieved { get; set; }
    }
}
