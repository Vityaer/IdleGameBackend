using UniverseRift.Models.Misc;

namespace UniverseRift.Controllers.Misc.Mails
{
    public interface IMailController
    {
        public Task GetPlayerSave(int playerId, CommunicationData communicationData);
    }
}
