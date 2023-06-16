namespace UniverseRift.Models.Heroes
{
    public class Hero
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public string HeroTemplateId { get; set; } = string.Empty;
        public string ViewId { get; set; } = string.Empty;
        public int Rating { get; set; }
        public int Level { get; set; }
        public string WeaponItemId { get; set; } = string.Empty;
        public string ArmorItemId { get; set; } = string.Empty;
        public string BootsItemId { get; set; } = string.Empty;
        public string AmuletItemId { get; set; } = string.Empty;

        public Hero()
        {
        }

        public Hero(int playerId, HeroTemplate heroTemplate)
        {
            PlayerId = playerId;
            HeroTemplateId = heroTemplate.Id;
            ViewId = heroTemplate.DefaultViewId;
            Rating = 1;
        }
    }
}
