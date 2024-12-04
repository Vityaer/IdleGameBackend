using UniverseRift.GameModels;
using UniverseRift.Models.Mines;

namespace UniverseRift.GameModelDatas.Cities.Industries
{
    public class IndustryData : BaseModel
    {
        public List<MineData> Mines = new();
        public List<MineMissionData> MissionDatas = new();
    }
}