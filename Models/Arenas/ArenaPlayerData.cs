using UniverseRift.GameModelDatas.Heroes;
using UniverseRift.GameModels;

namespace UniverseRift.Models.Arenas
{
    public class ArenaPlayerData
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int Score { get; set; }
        public int CurrentAreanaLevel { get; set; }
        public int MaxArenaLevel { get; set; }
        public string ArmyData { get; set; } = string.Empty;
        public int WinCount { get; set; }
        public int LoseCount { get; set; }
        public MissionModel Mission;
        public TeamContainer Team { get; set; }

        public ArenaPlayerData(ServerArenaPlayerData serverArenaPlayerData)
        {
            Id = serverArenaPlayerData.Id;
            PlayerId = serverArenaPlayerData.PlayerId;
            Score = serverArenaPlayerData.Score;
            CurrentAreanaLevel = serverArenaPlayerData.CurrentAreanaLevel;
        }
    }
}
