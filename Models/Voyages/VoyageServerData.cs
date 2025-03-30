using UniverseRift.GameModels;

namespace UniverseRift.Models.Voyages
{
    public class VoyageServerData
    {
        public int Id { get; set; }
        //public List<MissionModel> Missions { get; set; } = new();
        public bool IsVoyageDay { get; set; } = false;
    }
}
