using UniverseRift.Models.Results;

namespace UniverseRift.Controllers.Jsons
{
    public interface IJsonDataController
    {
        Task<AnswerModel> GetJsonFile(string name);
    }
}
