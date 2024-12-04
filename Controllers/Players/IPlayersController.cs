using UniverseRift.GameModelDatas.Players;
using UniverseRift.Models.Common;

namespace UniverseRift.Controllers.Players
{
    public interface IPlayersController
    {
        Task<Player> GetPlayer(int playerId);
        UniRx.IObservable<int> OnRegistrationPlayer { get; }
        Task<PlayerData> GetPlayerSave(int playerId);
        Task<Player> CreatePlayer(string name, string avatarPath, bool isBot);
    }
}
