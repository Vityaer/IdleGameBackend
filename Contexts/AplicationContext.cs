using Microsoft.EntityFrameworkCore;
using UniverseRift.GameModelDatas.AI;
using UniverseRift.GameModelDatas.Cities.Industries;
using UniverseRift.GameModelDatas.Cities.TravelCircleRaces;
using UniverseRift.GameModelDatas.Rewards;
using UniverseRift.Models.Achievments;
using UniverseRift.Models.Arenas;
using UniverseRift.Models.City.DailyRewards;
using UniverseRift.Models.City.Markets;
using UniverseRift.Models.Common;
using UniverseRift.Models.Common.Server;
using UniverseRift.Models.FortuneWheels;
using UniverseRift.Models.Guilds;
using UniverseRift.Models.Heroes;
using UniverseRift.Models.Inventories.Splinters;
using UniverseRift.Models.Items;
using UniverseRift.Models.LongTravels;
using UniverseRift.Models.Misc;
using UniverseRift.Models.Misc.Communications;
using UniverseRift.Models.Players;
using UniverseRift.Models.Resources;
using UniverseRift.Models.Rewards;
using UniverseRift.Models.Tasks.SimpleTask;
using UniverseRift.Models.Voyages;

namespace UniverseRift.Contexts
{
    public class AplicationContext : DbContext
    {
        private const string DATABASE_CONNECTION = "server=localhost;user=landelo;password=dS7wVuc&L5Nw;database=usersdb;";

        public DbSet<ServerLifeTime> ServerLifeTimes { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<PlayerProgress> PlayerProgresses { get; set; }
        public DbSet<Hero> Heroes { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Splinter> Splinters { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<GameTask> GameTasks { get; set; }
        public DbSet<FortuneWheelModel> FortuneWheels { get; set; }
        public DbSet<DailyRewardProgress> DailyRewardProgresses { get; set; }
        public DbSet<MineData> MineDatas { get; set; }
        public DbSet<TravelRaceData> TravelRaceDatas { get; set; }
        public DbSet<AchievmentData> AchievmentDatas { get; set; }
        public DbSet<BattlepasData> BattlepasDatas { get; set; }
        public DbSet<GuildData> GuildDatas { get; set; }
        public DbSet<GuildPlayerRequest> GuildPlayerRequests { get; set; }
        public DbSet<GuildPlayerBan> GuildPlayerBans { get; set; }
        public DbSet<LetterData> LetterDatas { get; set; }
        public DbSet<PlayerAsFriendData> PlayerAsFriendDatas { get; set; }
        public DbSet<FriendshipData> FriendshipDatas { get; set; }
        public DbSet<FriendshipRequest> FriendshipRequests { get; set; }
        public DbSet<PlayerBanRecord> PlayerBanRecords { get; set; }
        public DbSet<RecruitData> RecruitDatas { get; set; }
        public DbSet<VoyageData> VoyageDatas { get; set; }
        public DbSet<VoyageServerData> VoyageServerDatas { get; set; }
        public DbSet<ServerArenaPlayerData> ServerArenaPlayerDatas { get; set; }
        public DbSet<LongTravelServerData> LongTravelServerDatas { get; set; }
        public DbSet<MineMissionData> MineMissionDatas { get; set; }
        public DbSet<BotData> BotsDatas { get; set; }
        public DbSet<RewardServerData> RewardServerDatas { get; set; }
        public DbSet<ChatMessageData> ChatMessageDatas { get; set; }

        public AplicationContext(DbContextOptions<AplicationContext> options)
            : base(options)
        {
            Database.EnsureDeleted(); // удаление базы данных
            Database.EnsureCreated();   // создаем базу данных при первом обращении
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(DATABASE_CONNECTION,
                ServerVersion.AutoDetect(DATABASE_CONNECTION));
        }
    }
}
