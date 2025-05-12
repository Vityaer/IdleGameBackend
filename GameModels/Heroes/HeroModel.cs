
namespace UniverseRift.GameModels.Heroes
{
    public class HeroModel : BaseModel
    {
        public GeneralInfoHero General = new();
		public Characteristics Characteristics;
		public IncreaseCharacteristicsModel IncCharacts;
	}
}
