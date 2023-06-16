namespace Misc.Json
{
    public interface IJsonConverter
    {
        T FromJson<T>(string value);
        string ToJson<T>(T obj);
    }
}