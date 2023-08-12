using Newtonsoft.Json;

namespace Misc.Json.Impl
{
    public class JsonConverter : IJsonConverter
    {
        public string Serialize<T>(T obj)
        {
            string value = JsonConvert.SerializeObject(obj);
            return value;
        }
        public T Deserialize<T>(string value)
        {
            T obj = JsonConvert.DeserializeObject<T>(value);
            return obj;
        }
    }
}
