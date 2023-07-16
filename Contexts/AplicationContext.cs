using Microsoft.EntityFrameworkCore;
using UniverseRift.Misc;
using UniverseRift.Models.Common;
using UniverseRift.Models.Common.Server;
using UniverseRift.Models.Heroes;
using UniverseRift.Models.Inventories.Splinters;
using UniverseRift.Models.Items;
using UniverseRift.Models.Markets;
using UniverseRift.Models.Players;
using UniverseRift.Models.Resources;
using UniverseRift.Models.Rewards;
using UniverseRift.Models.Tasks.SimpleTask;

namespace UniverseRift.Contexts
{
    public class AplicationContext : DbContext
    {
        public DbSet<ServerLifeTime> ServerLifeTimes { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<PlayerProgress> PlayerProgresses { get; set; }
        public DbSet<HeroTemplate> HeroTemplates { get; set; }
        public DbSet<Hero> Heroes { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<ItemTemplate> ItemTemplates { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemSynthesisRelation> ItemSynthesisRelations { get; set; }
        public DbSet<Splinter> Splinters { get; set; }
        public DbSet<FortuneReward> FortuneRewards { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<GameTaskTemplate> GameTaskTemplates { get; set; }
        public DbSet<GameTask> GameTasks { get; set; }


        public AplicationContext(DbContextOptions<AplicationContext> options)
            : base(options)
        {
            //Database.EnsureDeleted(); // удаление базы данных
            Database.EnsureCreated();   // создаем базу данных при первом обращении
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

            modelBuilder.Entity<FortuneReward>().HasData(
                new FortuneReward { Id = 1, Type = TypeObject.Resource, Name = $"{ResourceType.Gold}", Count = 50f, E10 = 3, Posibility = 50 },
                new FortuneReward { Id = 2, Type = TypeObject.Resource, Name = $"{ResourceType.Diamond}", Count = 60, E10 = 0, Posibility = 50 },
                new FortuneReward { Id = 3, Type = TypeObject.Resource, Name = $"{ResourceType.SimpleHireCard}", Count = 5f, E10 = 0, Posibility = 50 },
                new FortuneReward { Id = 4, Type = TypeObject.Resource, Name = $"{ResourceType.SpecialHireCard}", Count = 1f, E10 = 0, Posibility = 50 },
                new FortuneReward { Id = 5, Type = TypeObject.Resource, Name = $"{ResourceType.ContinuumStone}", Count = 30f, E10 = 3, Posibility = 50 },
                new FortuneReward { Id = 6, Type = TypeObject.Resource, Name = $"{ResourceType.RedDust}", Count = 20f, E10 = 0, Posibility = 50 },
                new FortuneReward { Id = 7, Type = TypeObject.Resource, Name = $"{ResourceType.ForceStone}", Count = 80f, E10 = 0, Posibility = 50 },
                new FortuneReward { Id = 8, Type = TypeObject.Resource, Name = $"{ResourceType.SimpleTask}", Count = 2f, E10 = 0, Posibility = 50 }
            );
        }


    }
}
