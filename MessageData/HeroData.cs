using UniverseRift.Models.Heroes;

namespace UniverseRift.MessageData
{
    public class HeroData
    {
        public int Id;
        public string HeroId;
        public int Level = 1;
        public int Rating = 1;
        public int CurrentBreakthrough = 0;
        public CostumeData Costume = new CostumeData();

        public HeroData() { }

        public HeroData(Hero hero)
        {
            Id = hero.Id;
            HeroId = hero.HeroTemplateId;
            Level = hero.Level;
            Rating = hero.Rating;
        }
    }
}
