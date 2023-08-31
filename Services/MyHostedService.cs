using UniverseRift.Contexts;
using UniverseRift.Controllers.Buildings.Shops;
using UniverseRift.Controllers.Buildings.TaskBoards;
using UniverseRift.Controllers.Server;

namespace UniverseRift.Services
{
    public class MyHostedService : IHostedService
    {
        private readonly IServerController _serverController;
        public MyHostedService(
            IServerController serverController
            )
        {
            _serverController = serverController;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _serverController.OnStartProject();
            return Task.CompletedTask;
        }
        
        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Если нужно дождаться завершения очистки, но контролировать время, то стоит предусмотреть в контракте использование CancellationToken
            //await someService.DoSomeCleanupAsync(cancellationToken);
            return Task.CompletedTask;
        }
    }
}
