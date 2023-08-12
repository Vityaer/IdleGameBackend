using UniverseRift.Controllers.Buildings.Shops;
using UniverseRift.Controllers.Buildings.TaskBoards;
using UniverseRift.Controllers.Server;

namespace UniverseRift.Services
{
    public class MyHostedService : IHostedService
    {
        private readonly IServerController _serverController;
        private readonly IMarketController _marketController;
        private readonly ITaskBoardController _taskBoardController;
        public MyHostedService(
            IServerController someService, 
            IMarketController marketController,
            ITaskBoardController taskBoardController
            )
        {
            _taskBoardController = taskBoardController;
            _serverController = someService;
            _marketController = marketController;
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
