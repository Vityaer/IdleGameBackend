using UniverseRift.GameModels;
using UniverseRift.Models.City.Markets;

namespace Models.City.Markets
{
    public class MarketModel : BaseModel
    {
        public List<string> Products = new List<string>();
        public RecoveryType RecoveryType;
    }
}
