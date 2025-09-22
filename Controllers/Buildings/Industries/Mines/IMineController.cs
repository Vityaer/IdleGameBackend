namespace UniverseRift.Controllers.Buildings.Industries.Mines
{
    public interface IMineController
    {
        Task OnRegistrationPlayer(int playerId);
        Task RefreshMissions(int playerId);
    }
}
