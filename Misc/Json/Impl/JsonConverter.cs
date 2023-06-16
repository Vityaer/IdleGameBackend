using Newtonsoft.Json;

namespace Misc.Json.Impl
{
    public class JsonConverter : IJsonConverter
    {
        public string ToJson<T>(T obj)
        {
            string value = JsonConvert.SerializeObject(obj);
            return value;
        }
        public T FromJson<T>(string value)
        {
            T obj = JsonConvert.DeserializeObject<T>(value);
            return obj;
        }
    }
}
