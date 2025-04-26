using UniverseRift.Models.Guilds;
using UniverseRift.Models.Misc;

namespace UniverseRift.Controllers.Buildings.Guilds
{
    public interface IGuildController
    {
        Task<GuildPlayerSaveContainer> GetPlayerSave(int playerId, CommunicationData communicationData, bool flagCreateNewData);
        Task OnPlayerRegister(int playerId);
        Task RefreshDay();
    }
}
