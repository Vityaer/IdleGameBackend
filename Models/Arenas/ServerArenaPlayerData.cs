namespace UniverseRift.Models.Arenas
{
    public class ServerArenaPlayerData
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int Score { get; set; }
        public int CurrentAreanaLevel { get; set; }
        public int MaxArenaLevel { get; set; }
        public string ArmyData { get; set; } = string.Empty;
    }
}
