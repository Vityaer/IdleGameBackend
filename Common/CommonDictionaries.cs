using Misc.Json;
using Models.City.FortuneRewards;
using Models.City.Hires;
using Models.City.Markets;
using UniverseRift.Controllers.Misc.Fights;
using UniverseRift.GameModelDatas.Cities.Buildings;
using UniverseRift.GameModelDatas.Cities.Industries;
using UniverseRift.GameModels;
using UniverseRift.GameModels.Heroes;
using UniverseRift.GameModels.Items;
using UniverseRift.Misc;
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
    public class CommonDictionaries : ICommonDictionaries
    {
        private Dictionary<string, HeroModel> _heroes = new();
        private Dictionary<string, CampaignChapterModel> _campaignChapters = new();
        private Dictionary<string, CostLevelUpContainer> _heroesCostLevelUps = new();
        private Dictionary<string, RewardModel> _rewards = new();
        private Dictionary<string, ItemModel> _items = new();
        private Dictionary<string, ItemRelationModel> _itemRelations = new();
        private Dictionary<string, BaseProductModel> _products = new();
        //private Dictionary<string, PosibleObjectModel> _posibleObjects = new Dictionary<string, PosibleObjectModel>();
        private Dictionary<string, GameTaskModel> _gameTaskModels = new Dictionary<string, GameTaskModel>();
        private Dictionary<string, MarketModel> _markets = new Dictionary<string, MarketModel>();
        private Dictionary<string, FortuneRewardModel> _fortuneRewardModels = new Dictionary<string, FortuneRewardModel>();
        private Dictionary<string, CostLevelUpContainer> _costContainers = new Dictionary<string, CostLevelUpContainer>();
        private Dictionary<string, TravelRaceModel> _travelRaceCampaigns = new Dictionary<string, TravelRaceModel>();
        private Dictionary<string, SplinterModel> _splinters = new Dictionary<string, SplinterModel>();
        private Dictionary<string, StorageChallengeModel> _storageChallenges = new Dictionary<string, StorageChallengeModel>();

        private Dictionary<string, AchievmentContainerModel> _achievmentContainers = new();
        private Dictionary<string, AchievmentModel> _achievments = new();

        private Dictionary<string, MineModel> _mines = new Dictionary<string, MineModel>();
        private Dictionary<string, MineRestrictionModel> _mineRestrictions = new Dictionary<string, MineRestrictionModel>();
        private Dictionary<string, RewardContainerModel> _rewardContainerModels = new();
        private Dictionary<string, RatingUpContainer> _ratingUpContainers = new();
        private Dictionary<string, GuildBossContainer> _guildBossContainers = new();
        private Dictionary<string, AvatarModel> _avatarModels = new();
        private Dictionary<string, HireContainerModel> _hireContainerModels = new();
        private Dictionary<string, RaceModel> _races = new();
        private Dictionary<string, BuildingModel> _buildings = new();

        private readonly IJsonConverter _converter;
        private bool _isInited;

        public Dictionary<string, HeroModel> Heroes => _heroes;
        public Dictionary<string, CampaignChapterModel> CampaignChapters => _campaignChapters;
        public Dictionary<string, CostLevelUpContainer> HeroesCostLevelUps => _heroesCostLevelUps;
        public Dictionary<string, RewardModel> Rewards => _rewards;
        public Dictionary<string, ItemModel> Items => _items;
        public Dictionary<string, ItemRelationModel> ItemRelations => _itemRelations;
        public Dictionary<string, BaseProductModel> Products => _products;
        //public Dictionary<string, PosibleObjectModel> PosibleObjects => _posibleObjects;
        public Dictionary<string, MarketModel> Markets => _markets;
        public Dictionary<string, GameTaskModel> GameTaskModels => _gameTaskModels;
        public Dictionary<string, FortuneRewardModel> FortuneRewardModels => _fortuneRewardModels;
        public Dictionary<string, CostLevelUpContainer> CostContainers => _costContainers;
        public Dictionary<string, MineRestrictionModel> MineRestrictions => _mineRestrictions;
        public Dictionary<string, MineModel> Mines => _mines;
        public Dictionary<string, TravelRaceModel> TravelRaceCampaigns => _travelRaceCampaigns;
        public Dictionary<string, SplinterModel> Splinters => _splinters;
        public Dictionary<string, AchievmentContainerModel> AchievmentContainers => _achievmentContainers;
        public Dictionary<string, AchievmentModel> Achievments => _achievments;
        public Dictionary<string, RewardContainerModel> RewardContainerModels => _rewardContainerModels;
        public Dictionary<string, StorageChallengeModel> StorageChallenges => _storageChallenges;
        public Dictionary<string, RatingUpContainer> RatingUpContainers => _ratingUpContainers;
        public Dictionary<string, GuildBossContainer> GuildBossContainers => _guildBossContainers;
        public Dictionary<string, AvatarModel> AvatarModels => _avatarModels;
        public Dictionary<string, HireContainerModel> HireContainerModels => _hireContainerModels;
        public Dictionary<string, RaceModel> Races => _races;
        public Dictionary<string, BuildingModel> Buildings => _buildings;

        public CommonDictionaries(IJsonConverter converter)
        {
            _converter = converter;
            Init();
        }

        public void Init()
        {
            LoadFromLocalDirectory();
            _isInited = true;
        }

        private void LoadFromLocalDirectory()
        {
            _heroes = GetModels<HeroModel>();
            _campaignChapters = GetModels<CampaignChapterModel>();
            _heroesCostLevelUps = GetModels<CostLevelUpContainer>();
            _rewards = GetModels<RewardModel>();
            _items = GetModels<ItemModel>();
            _itemRelations = GetModels<ItemRelationModel>();
            _products = GetModels<BaseProductModel>();
            _markets = GetModels<MarketModel>();
            _gameTaskModels = GetModels<GameTaskModel>();
            _fortuneRewardModels = GetModels<FortuneRewardModel>();
            _costContainers = GetModels<CostLevelUpContainer>();
            _mineRestrictions = GetModels<MineRestrictionModel>();
            _mines = GetModels<MineModel>();
            _travelRaceCampaigns = GetModels<TravelRaceModel>();
            _splinters = GetModels<SplinterModel>();
            _achievments = GetModels<AchievmentModel>();
            _achievmentContainers = GetModels<AchievmentContainerModel>();
            _rewardContainerModels = GetModels<RewardContainerModel>();
            _storageChallenges = GetModels<StorageChallengeModel>();
            _ratingUpContainers = GetModels<RatingUpContainer>();
            _guildBossContainers = GetModels<GuildBossContainer>();
            _avatarModels = GetModels<AvatarModel>();
            _hireContainerModels = GetModels<HireContainerModel>();
            _races = GetModels<RaceModel>();
            _buildings = GetModels<BuildingModel>();
        }

        private Dictionary<string, T> GetModels<T>() where T : BaseModel
        {
            var jsonData = TextUtils.GetJsonFile<T>();
            return TextUtils.FillDictionary<T>(jsonData, _converter);
        }
    }
}
