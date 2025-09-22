using UniverseRift.GameModels;
using UniverseRift.Models.Mines;

namespace UniverseRift.GameModelDatas.Cities.Industries
{
    public class IndustryData : BaseModel
    {
        public List<MineData> Mines = new();
        public List<MineMissionData> MissionDatas = new();
        public MineMissionData BossMissionData = new();
		public int MineEnergy;
        public string DateTimeCreate;
	}
}