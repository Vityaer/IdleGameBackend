using UniverseRift.MessageData;

namespace UniverseRift.GameModelDatas.Players
{
    public class HeroesStorage : BaseDataModel
    {
        public List<HeroData> ListHeroes = new List<HeroData>();
        public int MaxCountHeroes;
    }
}