using UniverseRift.Models.Results;

namespace UniverseRift.Controllers.Players.Inventories.Splinters
{
    public interface ISplinterController
    {
        Task<AnswerModel> AddSplinters(int playerId, string splinterId, int amount);
        Task<AnswerModel> UseSplinters(int playerId, string splinterId, int count);
    }
}
