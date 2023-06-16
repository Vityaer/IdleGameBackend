using UniverseRift.Models.Heroes;

namespace UniverseRift.Controllers.Players.Heroes
{
    public interface IHeroesController
    {
        public Task<Hero> GetHero(int playerId, int heroId);
    }
}
