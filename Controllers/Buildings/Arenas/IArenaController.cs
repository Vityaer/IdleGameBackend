using UniverseRift.GameModelDatas.Cities.Buildings;
using UniverseRift.Models.Misc;
using UniverseRift.Models.Results;

namespace UniverseRift.Controllers.Buildings.Arenas
{
    public interface IArenaController
    {
		Task OnPlayerRegister(int playerId);

		Task OnStartServer();
        Task RefreshDay();

		Task<ArenaData> GetPlayerSave(int playerId, CommunicationData communicationData);
        Task<AnswerModel> SetDefenders(int playerId, string heroesIdsContainer);
    }
}
