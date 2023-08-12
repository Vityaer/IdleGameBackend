using UniRx;

namespace UniverseRift.Controllers.Server
{
    public interface IServerController
    {
        void OnStartProject();
        ReactiveCommand OnChangeDay { get; }
        ReactiveCommand OnChangeWeek { get; }
        ReactiveCommand OnChangeMonth { get; }
        ReactiveCommand OnChangeGameCycle { get; }
    }
}
