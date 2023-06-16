using UniverseRift.Models.Common;

namespace UniverseRift.Controllers.Players
{
    public interface IPlayersController
    {
        Task<Player> GetPlayer(int playerId);
    }
}
