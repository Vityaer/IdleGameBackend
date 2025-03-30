namespace UniverseRift.Models.FortuneWheels
{
    public class FortuneWheelModel
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int RefreshCount { get; set; }
        public string RewardsJson { get; set; } = string.Empty;
    }
}
