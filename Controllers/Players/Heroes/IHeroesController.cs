using UniverseRift.GameModelDatas.Players;
using UniverseRift.Models.Heroes;

namespace UniverseRift.Controllers.Players.Heroes
{
    public interface IHeroesController
    {
        Task<Hero> GetHero(int playerId, int heroId);
        Task<HeroesStorage> GetPlayerSave(int playerId);

        UniRx.IObservable<HireDataContainer> OnSimpleHire { get; }
        UniRx.IObservable<HireDataContainer> OnSpecialHire { get; }
        UniRx.IObservable<HireDataContainer> OnFriendHire { get; }
    }
}
