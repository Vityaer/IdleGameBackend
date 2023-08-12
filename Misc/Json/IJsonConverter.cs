namespace Misc.Json
{
    public interface IJsonConverter
    {
        T Deserialize<T>(string value);
        string Serialize<T>(T obj);
    }
}