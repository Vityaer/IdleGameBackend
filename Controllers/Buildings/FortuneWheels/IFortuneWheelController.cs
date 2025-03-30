using UniverseRift.Models.City.FortuneWheels;

namespace UniverseRift.Controllers.Buildings.FortuneWheels
{
    public interface IFortuneWheelController
    {
        Task RefreshDay();
        Task<FortuneWheelData> GetPlayerSave(int playerId, bool flagCreateNewData);
    }
}
