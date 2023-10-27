namespace UniverseRift.GameModelDatas.Cities.TravelCircleRaces
{
    public class TravelRaceData
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int MissionIndexCompleted { get; set; }
        public string RaceId { get; set; }

        public TravelRaceData(int playerId, string raceId)
        {
            PlayerId = playerId;
            MissionIndexCompleted = -1;
            RaceId = raceId;
        }
    }
}
