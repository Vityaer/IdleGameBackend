namespace UniverseRift.GameModelDatas.Rewards
{
    public class RewardServerData
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public string RewardJSON { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
    }
}
