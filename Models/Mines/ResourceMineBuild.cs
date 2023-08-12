using Models.Data.Inventories;

namespace UniverseRift.Models.Mines
{
    public class ResourceMineBuild : AbstractMineBuild
    {
        public ResourceData MaxStorage;
        public ResourceData Income;
        public string LastGetDateTime;
    }
}
