using UniverseRift.GameModelDatas.Cities.Industries;

namespace UniverseRift.Controllers.Buildings.Industries
{
    public interface IIndustryController
    {
        Task<IndustryData> GetPlayerSave(int playerId, bool flagCreateNewData);
    }
}
