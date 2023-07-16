using Microsoft.AspNetCore.Mvc;
using Misc.Json;
using System.Text;
using UniverseRift.Controllers.Common;

namespace UniverseRift.Controllers.Jsons
{
    public class JsonDataController : ControllerBase, IJsonDataController
    {
        private readonly IJsonConverter _jsonConverter;

        public JsonDataController(IJsonConverter jsonConverter)
        {
            _jsonConverter = jsonConverter;
        }

        [HttpPost]
        [Route("Json/GetJsonFile")]
        public string GetJsonFile(string name)
        {
            var answer = string.Empty;
            var path = GetDataPath(name);
            var text = System.IO.File.ReadAllLines(path, Encoding.Default);

            foreach (var line in text)
            {
                var a = line;
                a = a.Replace("\r", string.Empty);
                a = a.Replace("\n", string.Empty);
                a = a.Replace("\\", string.Empty);
                answer = string.Concat(answer, a);
            }
            return answer;
        }

        public static string GetDataPath(string name)
        {
            var path = Path.Combine(Constants.Common.DictionariesPath, $"{name}.json");
            return path;
        }
    }
}
