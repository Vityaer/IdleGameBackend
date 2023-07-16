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

        private float _sumAll = 0;

        //public float GetAllSum
        //{
        //    get
        //    {
        //        if (_sumAll > 0) { return _sumAll; }
        //        else
        //        {
        //            for (int i = 0; i < PosibilityObjectRewards.Count; i++) _sumAll += PosibilityObjectRewards[i].Posibility;
        //            return _sumAll;
        //        }

        //    }
        //}

        //public float PosibleNumObject(int num)
        //{
        //    if (_sumAll <= 0f) _sumAll = GetAllSum;
        //    return PosibilityObjectRewards[num].Posibility / _sumAll * 100f;
        //}

        //public RewardData GetReward(int tryCount)
        //{
        //    var result = new RewardData();
        //    foreach (var posibleItem in PosibilityObjectRewards)
        //    {
        //        switch (posibleItem)
        //        {
        //            case PosibleGameResource posibleGameResource:
        //                //result.Add(posibleGameResource.GetResource(tryCount));
        //                break;
        //        }
        //    }
        //    return result;
        //}
    }
}