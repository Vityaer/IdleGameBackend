using Models.Data.Inventories;
using UIController.Rewards.PosibleRewards;

namespace UniverseRift.GameModels
{
    [Serializable]
    public class PosibleRewardData
    {
        public List<PosibleObjectData<ResourceData>> Resources = new List<PosibleObjectData<ResourceData>>();
        public List<PosibleObjectData<ItemData>> Items = new List<PosibleObjectData<ItemData>>();
        public List<PosibleObjectData<SplinterData>> Splinters = new List<PosibleObjectData<SplinterData>>();

        public List<PosibleObjectData> Objects
        {
            get
            {
                var result = new List<PosibleObjectData>(Resources.Count + Items.Count + Splinters.Count);
                foreach (var item in Resources)
                    result.Add(item);

                foreach (var item in Items)
                    result.Add(item);

                foreach (var item in Splinters)
                    result.Add(item);

                return result;
            }
        }
    }
}