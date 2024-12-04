using UniverseRift.Models.Misc;

namespace UniverseRift.Controllers.Misc.Friendships
{
    public interface IFriendshipController
    {
        Task OnPlayerRegister(int playerId);
        Task GetPlayerSave(int playerId, CommunicationData result);
    }
}
