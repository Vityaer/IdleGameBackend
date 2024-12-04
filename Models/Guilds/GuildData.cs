namespace UniverseRift.Models.Guilds
{
    public class GuildData
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CurrentBoss { get; set; } = 0;
        public float BossHealthMantissa { get; set; }
        public int BossHealthE10 { get; set; }
        public int Level { get; set; } = 1;
        public int Expirience { get; set; } = 0;
        public int CurrentPlayerCount { get; set; } = 1;
        public int MaxPlayerCount { get; set; } = 16;
        public int LeaderId { get; set; }
        public string IconPath { get; set; } = string.Empty;
        public int DonateEvoleLevel { get; set; } = 1;
        public float StorageMantissa { get; set; } 
        public int StorageE10 { get; set; }
    }
}
