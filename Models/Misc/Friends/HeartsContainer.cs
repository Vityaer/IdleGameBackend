namespace UniverseRift.Models.Misc.Friends
{
    [Serializable]
    public class HeartsContainer
    {
        public List<int> SendedHeartIds = new();
        public List<int> ReceivedHeartIds = new();
    }
}
