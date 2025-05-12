
namespace UniverseRift.Models.Arenas
{
    public class ServerArenaPlayerData
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int Score { get; set; }
        public int MaxScore { get; set; }
        public string ArmyData { get; set; } = string.Empty;
        public int WinCount { get; set; }
        public int LoseCount { get; set; }

		public void Refresh()
		{
            Score = 1000;
		}
	}
}
