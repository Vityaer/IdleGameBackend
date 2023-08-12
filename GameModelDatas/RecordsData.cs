namespace UniverseRift.GameModelDatas
{
    public class RecordsData<T>
    {
        public Dictionary<string, T> Records = new Dictionary<string, T>();

        public void SetRecord(string key, T value)
        {
            if (Records.ContainsKey(key))
            {
                Records[key] = value;
            }
            else
            {
                Records.Add(key, value);
            }
        }

        public T GetRecord(string key, T defaultValue = default)
        {
            if (Records.ContainsKey(key))
            {
                return Records[key];
            }

            return defaultValue;
        }

        public bool CheckRecord(string key)
        {
            return Records.ContainsKey(key);
        }
    }
}
