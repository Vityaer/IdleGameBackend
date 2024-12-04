using Models.Data.Inventories;

namespace UniverseRift.GameModels.Heroes
{
    public class RatingUpContainer : BaseModel
    {
        public List<ResourceData> Cost = new();
        public List<RequirementHeroModel> RequirementHeroes = new();
    }
}
