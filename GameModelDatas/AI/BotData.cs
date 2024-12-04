namespace UniverseRift.GameModelDatas.AI
{
    public class BotData
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }

        public BotData()
        {
        }

        public BotData(int playerId)
        {
            this.PlayerId = playerId;
        }
    }
}
