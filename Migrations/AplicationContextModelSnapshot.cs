﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UniverseRift.Contexts;

#nullable disable

namespace UniverseRift.Migrations
{
    [DbContext(typeof(AplicationContext))]
    partial class AplicationContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("UniverseRift.Models.City.DailyRewards.DailyRewardProgress", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("OpenIndex")
                        .HasColumnType("int");

                    b.Property<int>("PlayerId")
                        .HasColumnType("int");

                    b.Property<int>("ReceivedIndex")
                        .HasColumnType("int");

                    b.Property<string>("RewardDataJSON")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("RewardId")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("DailyRewardProgresses");
                });

            modelBuilder.Entity("UniverseRift.Models.City.Markets.Purchase", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("PlayerId")
                        .HasColumnType("int");

                    b.Property<string>("ProductId")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("PurchaseCount")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Purchases");
                });

            modelBuilder.Entity("UniverseRift.Models.Common.Player", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("GuildId")
                        .HasColumnType("int");

                    b.Property<string>("LastEnteredDateTime")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("LastUpdateGameData")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("Level")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("ServerId")
                        .HasColumnType("int");

                    b.Property<int>("VipLevel")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("UniverseRift.Models.Common.Server.ServerLifeTime", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("LastStartDateTime")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("NexGameCycle")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("NextDay")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("NextMonth")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("NextWeek")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("ServerLifeTimes");
                });

            modelBuilder.Entity("UniverseRift.Models.FortuneWheels.FortuneWheelModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("PlayerId")
                        .HasColumnType("int");

                    b.Property<string>("RewardsJson")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("FortuneWheels");
                });

            modelBuilder.Entity("UniverseRift.Models.Heroes.Hero", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("AmuletItemId")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("ArmorItemId")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("BootsItemId")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("HeroTemplateId")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("Level")
                        .HasColumnType("int");

                    b.Property<int>("PlayerId")
                        .HasColumnType("int");

                    b.Property<int>("Rating")
                        .HasColumnType("int");

                    b.Property<string>("ViewId")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("WeaponItemId")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Heroes");
                });

            modelBuilder.Entity("UniverseRift.Models.Heroes.HeroTemplate", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("DefaultViewId")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<byte>("Race")
                        .HasColumnType("tinyint unsigned");

                    b.Property<int>("Rare")
                        .HasColumnType("int");

                    b.Property<int>("Rating")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("HeroTemplates");

                    b.HasData(
                        new
                        {
                            Id = "DeathKnight",
                            DefaultViewId = "DeathKnight",
                            Race = (byte)2,
                            Rare = 0,
                            Rating = 1
                        },
                        new
                        {
                            Id = "Demoniac",
                            DefaultViewId = "Demoniac",
                            Race = (byte)2,
                            Rare = 0,
                            Rating = 1
                        },
                        new
                        {
                            Id = "Imp",
                            DefaultViewId = "Imp",
                            Race = (byte)2,
                            Rare = 0,
                            Rating = 1
                        },
                        new
                        {
                            Id = "Legolas",
                            DefaultViewId = "Legolas",
                            Race = (byte)2,
                            Rare = 0,
                            Rating = 1
                        },
                        new
                        {
                            Id = "Militia",
                            DefaultViewId = "Militia",
                            Race = (byte)2,
                            Rare = 0,
                            Rating = 1
                        },
                        new
                        {
                            Id = "Peasant",
                            DefaultViewId = "Peasant",
                            Race = (byte)2,
                            Rare = 0,
                            Rating = 1
                        },
                        new
                        {
                            Id = "Pegasus",
                            DefaultViewId = "Pegasus",
                            Race = (byte)2,
                            Rare = 0,
                            Rating = 1
                        },
                        new
                        {
                            Id = "Raccoon",
                            DefaultViewId = "Raccoon",
                            Race = (byte)2,
                            Rare = 0,
                            Rating = 1
                        },
                        new
                        {
                            Id = "Robin",
                            DefaultViewId = "Robin",
                            Race = (byte)2,
                            Rare = 0,
                            Rating = 1
                        },
                        new
                        {
                            Id = "Spirit",
                            DefaultViewId = "Spirit",
                            Race = (byte)2,
                            Rare = 0,
                            Rating = 1
                        },
                        new
                        {
                            Id = "TempleGuard",
                            DefaultViewId = "TempleGuard",
                            Race = (byte)2,
                            Rare = 0,
                            Rating = 1
                        });
                });

            modelBuilder.Entity("UniverseRift.Models.Inventories.Splinters.Splinter", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<float>("Count")
                        .HasColumnType("float");

                    b.Property<int>("E10")
                        .HasColumnType("int");

                    b.Property<int>("PlayerId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Splinters");
                });

            modelBuilder.Entity("UniverseRift.Models.Items.Item", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<float>("Count")
                        .HasColumnType("float");

                    b.Property<int>("E10")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("PlayerId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Items");
                });

            modelBuilder.Entity("UniverseRift.Models.Players.PlayerProgress", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("CampaignProgress")
                        .HasColumnType("int");

                    b.Property<int>("ChellangeTowerProgress")
                        .HasColumnType("int");

                    b.Property<string>("LastGetAutoFightReward")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("PlayerId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("PlayerProgresses");
                });

            modelBuilder.Entity("UniverseRift.Models.Resources.Resource", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<float>("Count")
                        .HasColumnType("float");

                    b.Property<int>("E10")
                        .HasColumnType("int");

                    b.Property<int>("PlayerId")
                        .HasColumnType("int");

                    b.Property<byte>("Type")
                        .HasColumnType("tinyint unsigned");

                    b.HasKey("Id");

                    b.ToTable("Resources");
                });

            modelBuilder.Entity("UniverseRift.Models.Tasks.SimpleTask.GameTask", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("DateTimeCreate")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("DateTimeStart")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("PlayerId")
                        .HasColumnType("int");

                    b.Property<float>("RewardFactor")
                        .HasColumnType("float");

                    b.Property<byte>("Status")
                        .HasColumnType("tinyint unsigned");

                    b.Property<string>("TaskModelId")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("GameTasks");
                });
#pragma warning restore 612, 618
        }
    }
}