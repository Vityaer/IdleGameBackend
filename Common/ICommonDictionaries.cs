using Models.City.FortuneRewards;
using Models.City.Markets;
using UniverseRift.GameModelDatas.Cities.Industries;
using UniverseRift.GameModels;
using UniverseRift.GameModels.Items;
using UniverseRift.Models.City.Mines;
using UniverseRift.Models.Tasks.SimpleTask;

namespace UniverseRift.Controllers.Common
{
    public interface ICommonDictionaries
    {
        Dictionary<string, CampaignChapterModel> CampaignChapters { get; }
        Dictionary<string, CostLevelUpContainer> HeroesCostLevelUps { get; }
        Dictionary<string, RewardModel> Rewards { get; }
        Dictionary<string, ItemModel> Items { get; }
        Dictionary<string, ItemRelationModel> ItemRelations { get; }
        Dictionary<string, BaseProductModel> Products { get; }
        //Dictionary<string, PosibleObjectModel> PosibleObjects { get; }
        Dictionary<string, MarketModel> Markets { get; }
        Dictionary<string, GameTaskModel> GameTaskModels { get; }
        Dictionary<string, FortuneRewardModel> FortuneRewardModels { get; }
        Dictionary<string, CostLevelUpContainer> CostContainers { get; }
        Dictionary<string, MineRestrictionModel> MineRestrictions { get; }
        Dictionary<string, MineModel> Mines { get; }

    }
}
