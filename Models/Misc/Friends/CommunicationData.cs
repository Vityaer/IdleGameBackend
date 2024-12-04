using UniverseRift.GameModelDatas.Players;
using UniverseRift.Models.Misc.Communications;

namespace UniverseRift.Models.Misc
{
    public class CommunicationData
    {
        public List<FriendshipData> FriendshipDatas = new();
        public List<FriendshipRequest> FriendshipRequests = new();
        public Dictionary<int, PlayerData> PlayersData = new();
        public List<LetterData> LetterDatas = new();
        public List<ChatMessageData> ChatMessages = new();

        public void AddPlayerData(PlayerData playerData)
        {
            if (playerData == null)
                return;

            if (PlayersData.ContainsKey(playerData.Id))
                return;

            PlayersData.Add(playerData.Id, playerData);
        }
    }
}
