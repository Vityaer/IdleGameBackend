using Microsoft.AspNetCore.Mvc;
using Misc.Json;
using System.Text;
using UniverseRift.Controllers.Common;
using UniverseRift.Models.Results;

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
        public async Task<AnswerModel> GetJsonFile(string name)
        {
            var answer = new AnswerModel();
            var jsonText = string.Empty;
            var path = GetDataPath(name);
            var text = System.IO.File.ReadAllLines(path, Encoding.Latin1);

            foreach (var line in text)
            {
                var a = line;
                a = a.Replace("\r", string.Empty);
                a = a.Replace("\n", string.Empty);
                a = a.Replace("\\", string.Empty);
                //a = a.Replace("\"", string.Empty);
                jsonText = string.Concat(jsonText, a);
            }

            answer.Result = jsonText;
            return answer;
        }

        [HttpPost]
        [Route("Json/GetJsonTextFile")]
        public string GetJsonTextFile(string name)
        {
            var jsonText = string.Empty;
            var path = GetDataPath(name);
            var text = System.IO.File.ReadAllLines(path, Encoding.Latin1);

            foreach (var line in text)
            {
                var a = line;
                a = a.Replace("\r", string.Empty);
                a = a.Replace("\n", string.Empty);
                a = a.Replace("\\", string.Empty);
                //a = a.Replace("\"", string.Empty);
                jsonText = string.Concat(jsonText, a);
            }

            return jsonText;
        }

        [HttpPost]
        [Route("Json/GetFile")]
        public IActionResult GetFile(string name)
        {
            string file_path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, GetDataPath(name));
            // Путь к файлу
            //string file_path = GetDataPath(name);
            // Тип файла - content-type
            string file_type = "application/json";
            // Имя файла - необязательно
            string file_name = $"{name}.json";
            return PhysicalFile(file_path, file_type, file_name);
        }
        public static string GetDataPath(string name)
        {
            var path = Path.Combine(Constants.Common.DictionariesPath, $"{name}.json");
            return path;
        }
    }
}
