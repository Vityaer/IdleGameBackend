using Models.City.FortuneRewards;
using Models.City.Hires;
using Models.City.Markets;
using UniverseRift.Controllers.Misc.Fights;
using UniverseRift.GameModelDatas.Cities.Buildings;
using UniverseRift.GameModelDatas.Cities.Industries;
using UniverseRift.GameModels;
using UniverseRift.GameModels.Heroes;
using UniverseRift.GameModels.Items;
using UniverseRift.Models.Achievments;
using UniverseRift.Models.City.Mines;
using UniverseRift.Models.City.TravelCircles;
using UniverseRift.Models.Guilds;
using UniverseRift.Models.Heroes;
using UniverseRift.Models.Inventories.Splinters;
using UniverseRift.Models.Players.Avatars;
using UniverseRift.Models.Rewards;
using UniverseRift.Models.Tasks.SimpleTask;

namespace UniverseRift.Controllers.Common
{
    public interface ICommonDictionaries
    {
        Dictionary<string, HeroModel> Heroes { get; }
        Dictionary<string, CampaignChapterModel> CampaignChapters { get; }
        Dictionary<string, CostLevelUpContainer> HeroesCostLevelUps { get; }
        Dictionary<string, RewardModel> Rewards { get; }
        Dictionary<string, ItemModel> Items { get; }
        Dictionary<string, ItemRelationModel> ItemRelations { get; }
        Dictionary<string, BaseProductModel> Products { get; }
        Dictionary<string, MarketModel> Markets { get; }
        Dictionary<string, GameTaskModel> GameTaskModels { get; }
        Dictionary<string, FortuneRewardModel> FortuneRewardModels { get; }
        Dictionary<string, CostLevelUpContainer> CostContainers { get; }
        Dictionary<string, MineRestrictionModel> MineRestrictions { get; }
        Dictionary<string, MineModel> Mines { get; }
        Dictionary<string, TravelRaceModel> TravelRaceCampaigns { get; }
        Dictionary<string, SplinterModel> Splinters { get; }
        Dictionary<string, AchievmentContainerModel> AchievmentContainers { get; }
        Dictionary<string, AchievmentModel> Achievments { get; }
        Dictionary<string, RewardContainerModel> RewardContainerModels { get; }
        Dictionary<string, StorageChallengeModel> StorageChallenges { get; }
        Dictionary<string, RatingUpContainer> RatingUpContainers { get; }
        Dictionary<string, GuildBossContainer> GuildBossContainers { get; }
        Dictionary<string, AvatarModel> AvatarModels { get; }
        Dictionary<string, HireContainerModel> HireContainerModels { get; }
        Dictionary<string, RaceModel> Races { get; }
        Dictionary<string, BuildingModel> Buildings { get; }
    }
}
