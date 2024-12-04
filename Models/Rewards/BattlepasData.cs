namespace UniverseRift.Models.Rewards
{
    public class BattlepasData
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int CurrentDailyBattlepasStage = -1;

        public BattlepasData(int playerId)
        {
            PlayerId = playerId;
        }
    }
}
