using UniverseRift.GameModels;
using UniverseRift.Models.Misc;

namespace UniverseRift.Controllers.Misc.Mails
{
    public interface IMailController
    {
        Task GetPlayerSave(int playerId, CommunicationData communicationData);
        Task CreateLetterWithReward(int playerId, string topicKey, string messageKey, RewardModel rewardModel);
	}
}
