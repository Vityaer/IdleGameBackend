using UniverseRift.Models.LongTravels;

namespace UniverseRift.Controllers.Buildings.LongTravels
{
    public interface ILongTravelController
    {
        Task<LongTravelData> GetPlayerSave(int playerId, bool flagCreateNewData);
        Task OnPlayerRegister(int playerId);
    }
}
