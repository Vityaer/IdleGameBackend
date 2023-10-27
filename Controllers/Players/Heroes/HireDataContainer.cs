namespace UniverseRift.Controllers.Players.Heroes
{
    public class HireDataContainer
    {
        public int Amount;
        public int PlayerId;

        public HireDataContainer(int playerId, int count)
        {
            PlayerId = playerId;
            Amount = count;
        }
    }
}
