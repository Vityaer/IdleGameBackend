using Misc.Json;
using Models.City.FortuneRewards;
using Models.City.Markets;
using UniverseRift.GameModels;
using UniverseRift.GameModels.Items;
using UniverseRift.Models.Tasks.SimpleTask;

namespace UniverseRift.Controllers.Common
{
    public class CommonDictionaries : ICommonDictionaries
    {
        private Dictionary<string, CampaignChapterModel> _campaignChapters = new Dictionary<string, CampaignChapterModel>();
        private Dictionary<string, CostLevelUpContainer> _heroesCostLevelUps = new Dictionary<string, CostLevelUpContainer>();
        private Dictionary<string, RewardModel> _rewards = new Dictionary<string, RewardModel>();
        private Dictionary<string, ItemModel> _items = new Dictionary<string, ItemModel>();
        private Dictionary<string, ItemRelationModel> _itemRelations = new Dictionary<string, ItemRelationModel>();
        private Dictionary<string, BaseProductModel> _products = new Dictionary<string, BaseProductModel>();
        //private Dictionary<string, PosibleObjectModel> _posibleObjects = new Dictionary<string, PosibleObjectModel>();
        private Dictionary<string, GameTaskModel> _gameTaskModels = new Dictionary<string, GameTaskModel>();
        private Dictionary<string, MarketModel> _markets = new Dictionary<string, MarketModel>();
        private Dictionary<string, FortuneRewardModel> _fortuneRewardModels = new Dictionary<string, FortuneRewardModel>();
        private readonly IJsonConverter _converter;
        private bool _isInited;

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
            _campaignChapters = GetModels<CampaignChapterModel>();
            _heroesCostLevelUps = GetModels<CostLevelUpContainer>();
            _rewards = GetModels<RewardModel>();
            _items = GetModels<ItemModel>();
            _itemRelations = GetModels<ItemRelationModel>();
            _products = GetModels<BaseProductModel>();
            _markets = GetModels<MarketModel>();
            _gameTaskModels = GetModels<GameTaskModel>();
            _fortuneRewardModels = GetModels<FortuneRewardModel>();
            //_posibleObjects = GetModels<PosibleObjectModel>();
        }

        private Dictionary<string, T> GetModels<T>() where T : BaseModel
        {
            var jsonData = TextUtils.GetJsonFile<T>();
            return TextUtils.FillDictionary<T>(jsonData, _converter);
        }
    }
}
