using UniverseRift.GameModelDatas.Players;
using UniverseRift.Models.Arenas;

namespace UniverseRift.GameModelDatas.Cities.Buildings
{
    public class ArenaBuildingModel
    {
        public ArenaPlayerData MyData;
        public List<ArenaPlayerData> Opponents = new();
        public Dictionary<int, PlayerData> PlayersData = new();
    }
}