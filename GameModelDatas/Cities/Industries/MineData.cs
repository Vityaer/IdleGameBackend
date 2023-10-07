namespace UniverseRift.GameModelDatas.Cities.Industries
{
    public class MineData
    {
        public int Id { get; set; }
        public string MineId { get; set; } = string.Empty;
        public string PlaceId { get; set; } = string.Empty;
        public int PlayerId { get; set; }
        public int Level { get; set; }
        public string LastDateTimeGetIncome { get; set; } = string.Empty;

        public MineData() { }

        public MineData(int playerId, string mineId, string placeId)
        {
            PlayerId = playerId;
            Level = 1;
            MineId = mineId;
            PlaceId = placeId;
            LastDateTimeGetIncome = DateTime.UtcNow.ToString();
        }
    }
}