using Microsoft.EntityFrameworkCore;
using UniverseRift.GameModelDatas.Cities.Industries;
using UniverseRift.GameModelDatas.Cities.TravelCircleRaces;
using UniverseRift.Misc;
using UniverseRift.Models.Achievments;
using UniverseRift.Models.City.DailyRewards;
using UniverseRift.Models.City.Markets;
using UniverseRift.Models.Common;
using UniverseRift.Models.Common.Server;
using UniverseRift.Models.FortuneWheels;
using UniverseRift.Models.Heroes;
using UniverseRift.Models.Inventories.Splinters;
using UniverseRift.Models.Items;
using UniverseRift.Models.Mines;
using UniverseRift.Models.Players;
using UniverseRift.Models.Resources;
using UniverseRift.Models.Rewards;
using UniverseRift.Models.Tasks.SimpleTask;

namespace UniverseRift.Contexts
{
    public class AplicationContext : DbContext
    {
        private const string DATABASE_CONNECTION = "server=localhost;user=landelo;password=dS7wVuc&L5Nw;database=usersdb;";
        
        public DbSet<ServerLifeTime> ServerLifeTimes { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<PlayerProgress> PlayerProgresses { get; set; }
        public DbSet<HeroTemplate> HeroTemplates { get; set; }
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
        public DbSet<AchievmentData> MainAchievmentDatas { get; set; }
        public DbSet<AchievmentData> DailyTaskDatas { get; set; }
        public DbSet<AchievmentData> GameCycleTaskDatas { get; set; }
        public DbSet<BattlepasData> BattlepasDatas { get; set; }
        
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HeroTemplate>().HasData(
                    new HeroTemplate { Id = "DeathKnight", Rare = Rare.C, DefaultViewId = "DeathKnight", Race = Race.Elfs },
                    new HeroTemplate { Id = "Demoniac", Rare = Rare.C, DefaultViewId = "Demoniac", Race = Race.Elfs },
                    new HeroTemplate { Id = "Imp", Rare = Rare.C, DefaultViewId = "Imp", Race = Race.Elfs },
                    new HeroTemplate { Id = "Legolas", Rare = Rare.C, DefaultViewId = "Legolas", Race = Race.Elfs },
                    new HeroTemplate { Id = "Militia", Rare = Rare.C, DefaultViewId = "Militia", Race = Race.Elfs },
                    new HeroTemplate { Id = "Peasant", Rare = Rare.C, DefaultViewId = "Peasant", Race = Race.Elfs },
                    new HeroTemplate { Id = "Pegasus", Rare = Rare.C, DefaultViewId = "Pegasus", Race = Race.Elfs },
                    new HeroTemplate { Id = "Raccoon", Rare = Rare.C, DefaultViewId = "Raccoon", Race = Race.Elfs },
                    new HeroTemplate { Id = "Robin", Rare = Rare.C, DefaultViewId = "Robin", Race = Race.Elfs },
                    new HeroTemplate { Id = "Spirit", Rare = Rare.C, DefaultViewId = "Spirit", Race = Race.Elfs },
                    new HeroTemplate { Id = "TempleGuard", Rare = Rare.C, DefaultViewId = "TempleGuard", Race = Race.Elfs }
            );
        }
    }
}
