using UniverseRift.GameModelDatas.Cities.Industries;

namespace UniverseRift.Controllers.Buildings.Industries
{
    public interface IIndustryController
    {
        async Task<IndustryModel> GetPlayerSave(int playerId) { return null; }
    }
}
