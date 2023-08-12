using UniverseRift.Controllers.Players.Inventories.Items;
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
            
            Costume = new CostumeData();
            if (!string.IsNullOrEmpty(hero.WeaponItemId))
                Costume.Items.Add(ItemType.Weapon, hero.WeaponItemId);

            if (!string.IsNullOrEmpty(hero.ArmorItemId))
                Costume.Items.Add(ItemType.Armor, hero.ArmorItemId);

            if (!string.IsNullOrEmpty(hero.AmuletItemId))
                Costume.Items.Add(ItemType.Amulet, hero.AmuletItemId);

            if (!string.IsNullOrEmpty(hero.BootsItemId))
                Costume.Items.Add(ItemType.Boots, hero.BootsItemId);
        }
    }
}
