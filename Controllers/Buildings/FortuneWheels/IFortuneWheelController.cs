using UniverseRift.Models.City.FortuneWheels;

namespace UniverseRift.Controllers.Buildings.FortuneWheels
{
    public interface IFortuneWheelController
    {
        Task<FortuneWheelData> GetPlayerSave(int playerId);
    }
}
