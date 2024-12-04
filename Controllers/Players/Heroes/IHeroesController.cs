using UniverseRift.GameModelDatas.Players;
using UniverseRift.Models.Heroes;
using UniverseRift.Models.Results;

namespace UniverseRift.Controllers.Players.Heroes
{
    public interface IHeroesController
    {
        Task<Hero> GetHero(int playerId, int heroId);
        Task<HeroesStorage> GetPlayerSave(int playerId);
        Task<AnswerModel> GetSimpleHeroes(int playerId, int count);
    }
}
