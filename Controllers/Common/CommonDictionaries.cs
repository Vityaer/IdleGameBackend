using Misc.Json;
using UniRx;
using UniverseRift.GameModels;

namespace UniverseRift.Controllers.Common
{
    public class CommonDictionaries : ICommonDictionaries
    {
        public ReactiveCommand OnStartDownloadFiles = new ReactiveCommand();
        public ReactiveCommand OnFinishDownloadFiles = new ReactiveCommand();

        private Dictionary<string, CampaignChapterModel> _campaignChapters = new Dictionary<string, CampaignChapterModel>();
        private Dictionary<string, CostLevelUpContainer> _heroesCostLevelUps = new Dictionary<string, CostLevelUpContainer>();
        private Dictionary<string, RewardModel> _rewards = new Dictionary<string, RewardModel>();

        private readonly IJsonConverter _converter;
        private bool _isInited;

        public Dictionary<string, CampaignChapterModel> CampaignChapters => _campaignChapters;
        public Dictionary<string, CostLevelUpContainer> HeroesCostLevelUps => _heroesCostLevelUps;
        public Dictionary<string, RewardModel> Rewards => _rewards;


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
        }

        private Dictionary<string, T> GetModels<T>() where T : BaseModel
        {
            var jsonData = TextUtils.GetTextFromLocalStorage<T>();
            return TextUtils.FillDictionary<T>(jsonData, _converter);
        }
    }
}
