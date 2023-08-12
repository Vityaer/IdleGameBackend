using UniverseRift.GameModels;

namespace UniverseRift.GameModelDatas.Cities.Industries
{
    public class IndustryModel : BaseModel
    {
        public List<MineBuildModel> listAdminMine = new List<MineBuildModel>();
        public List<MineModel> listMine = new List<MineModel>();
    }
}