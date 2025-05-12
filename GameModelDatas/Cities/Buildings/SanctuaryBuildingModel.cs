using Models.Data.Inventories;
using UniverseRift.GameModelDatas.Cities.Buildings;

namespace Models.City.Sanctuaries
{
	public class SanctuaryBuildingModel : BuildingModel
	{
		public List<ResourceData> SimpleReplaceResource = new();
		public List<ResourceData> ConcreteReplaceResource = new();
	}
}
