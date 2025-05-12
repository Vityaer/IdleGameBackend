using UniverseRift.Controllers.Buildings.Arenas;
using UniverseRift.GameModelDatas.Cities.Buildings;
using UniverseRift.Models.Arenas;

namespace Models.City.Arena
{
	public class ArenaBuildingModel : BuildingModel
	{
		public Dictionary<ArenaType, ArenaContainerModel> ArenaContainers = new();
	}
}