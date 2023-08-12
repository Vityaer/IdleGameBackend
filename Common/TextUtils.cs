using Misc.Json;
using Newtonsoft.Json;
using System.Reflection;
using System;
using System.Text;
using UniverseRift.GameModels;

namespace UniverseRift.Controllers.Common
{
    public static class TextUtils
    {
        public static string GetConfigPath<T>()
        {
            var path = Path.Combine(Constants.Common.DictionariesPath, $"{typeof(T).Name}.json");
            return path;
        }

        public static Dictionary<string, T> FillDictionary<T>(string jsonData, IJsonConverter converter)
                   where T : BaseModel
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            var fromJson = JsonConvert.DeserializeObject<List<T>>(jsonData, settings);
            var result = new Dictionary<string, T>();

            if (fromJson == null)
                return result;

            foreach (var model in fromJson)
            {
                result.Add(model.Id, model);
            }

            return result;
        }

        public static string GetJsonFile<T>()
        {
            var path = GetDataPath(typeof(T).Name);
            var text = File.ReadAllText(path);

            text = text.Replace("\r", string.Empty);
            text = text.Replace("\n", string.Empty);
            text = text.Replace("\\", string.Empty);
            text = text.Replace("Assembly-CSharp", "UniverseRift");
            

            return text;
        }

        public static string GetDataPath(string name)
        {
            var path = Path.Combine(Constants.Common.DictionariesPath, $"{name}.json");
            return path;
        }
    }
}