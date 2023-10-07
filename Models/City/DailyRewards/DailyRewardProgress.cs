namespace UniverseRift.Models.City.DailyRewards
{
    public class DailyRewardProgress
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public string RewardId { get; set; } = string.Empty;
        public string RewardDataJSON { get; set; } = string.Empty;
        public int OpenIndex { get; set; } = 0;
        public int ReceivedIndex { get; set; } = -1;
    }
}
