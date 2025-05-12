using Models.Arenas;
using UniverseRift.GameModelDatas.Players;
using UniverseRift.Models.Arenas;

namespace UniverseRift.GameModelDatas.Cities.Buildings
{
    public class ArenaData
    {
        public ArenaPlayerData MyData;
        public List<ArenaPlayerData> Opponents = new();
        public Dictionary<int, PlayerData> PlayersData = new();
		public ArenaGeneralData ArenaGeneralData = new();
	}
}