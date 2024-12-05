using Cysharp.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using UniverseRift.Contexts;
using UniverseRift.Controllers.Bots;
using UniverseRift.Controllers.Buildings.Achievments;
using UniverseRift.Controllers.Buildings.GameCycles;
using UniverseRift.Controllers.Buildings.Guilds;
using UniverseRift.Controllers.Buildings.Shops;
using UniverseRift.Controllers.Buildings.TaskBoards;
using UniverseRift.Controllers.Buildings.Voyages;
using UniverseRift.Controllers.Common;
using UniverseRift.Heplers.GameLogging;
using UniverseRift.Models.City.Markets;
using UniverseRift.Models.Common.Server;
using UniverseRift.Models.Events;

namespace UniverseRift.Controllers.Server
{
    public class ServerController : ControllerBase, IServerController, IDisposable
    {
        private readonly AplicationContext _context;

        private readonly IMarketController _marketController;
        private readonly ITaskBoardController _taskBoardController;
        private readonly IAchievmentController _achievmentController;
        private readonly IVoyageController _voyageController;
        private readonly IGameCycleController _gameCycleController;
        private readonly IGuildController _guildController;
        private readonly IBotController _botController;

        private readonly TimeSpan Day = new TimeSpan(24, 0, 0);
        private readonly TimeSpan Week = new TimeSpan(7, 0, 0, 0);
        private readonly TimeSpan Month = new TimeSpan(30, 0, 0, 0);
        private readonly TimeSpan GameCycle = new TimeSpan(5, 0, 0, 0);

        private ServerLifeTime _server;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isCreated = false;

        public ServerController(
            AplicationContext context,
            IMarketController marketController,
            ITaskBoardController taskBoardController,
            IAchievmentController achievmentController,
            IVoyageController voyageController,
            IGameCycleController gameCycleController,
            IGuildController guildController,
            IBotController botController
            )
        {
            _context = context;
            _cancellationTokenSource = new CancellationTokenSource();
            _marketController = marketController;
            _taskBoardController = taskBoardController;
            _achievmentController = achievmentController;
            _voyageController = voyageController;
            _gameCycleController = gameCycleController;
            _guildController = guildController;
            _botController = botController;
        }

        public async Task OnStartProject()
        {
            if (_isCreated)
                return;

            _isCreated = true;
            await Start(_cancellationTokenSource.Token);
        }

        private async Task Start(CancellationToken cancellationToken)
        {
            var servers = await _context.ServerLifeTimes.ToListAsync();

            if (servers.Count == 0)
            {
                _server = new ServerLifeTime() { Id = 1, EventType = GameEventType.Sweet };
                var now = DateTime.UtcNow;
                _server.LastStartDateTime = now.ToString();

                var extraTime = new TimeSpan(0, now.Hour, now.Minute, now.Second);
                var startCurrentDay = now.Subtract(extraTime);
                _server.NextDay = startCurrentDay.Add(Day).ToString(Constants.Common.DateTimeFormat);
                _server.NextWeek = startCurrentDay.Add(Week).ToString(Constants.Common.DateTimeFormat);
                _server.NextMonth = startCurrentDay.Add(Month).ToString(Constants.Common.DateTimeFormat);
                _server.NexGameCycle = startCurrentDay.Add(GameCycle).ToString(Constants.Common.DateTimeFormat);

                await _context.ServerLifeTimes.AddAsync(_server);
                await _botController.OnStartServer();
                await _marketController.OnStartServer();
                await _context.SaveChangesAsync();
            }
            else
            {
                _server = servers[0];
            }

            WaitTime(DelayType.Day, Day, OnChangeDay, cancellationToken).Forget();
            WaitTime(DelayType.GameCycle, GameCycle, OnChangeGameCycle, cancellationToken).Forget();

            _gameCycleController.SetChangeCycle(_server.EventType);
        }

        private async Task OnChangeDay()
        {
            GameLogging.WriteGameLog($"Start refresh all city");
            await _marketController.RefreshProducts(RecoveryType.Day);
            await _taskBoardController.DeleteTasks();
            await _achievmentController.RefreshDailyTask();
            await _voyageController.NextDay();
            await _guildController.RefreshDay();

            GameLogging.WriteGameLog($"finish refresh all city");
        }

        private async Task OnChangeGameCycle()
        {
            var servers = await _context.ServerLifeTimes.ToListAsync();
            _server = servers[0];

            if (_server == null)
                return;

            GameLogging.WriteGameLog($"Next game cycle");
            var currentEventIndex = (int)_server.EventType;
            var nextEventIndex = currentEventIndex + 1;
            var listEventsCount = Enum.GetValues(typeof(GameEventType)).Length;
            if (nextEventIndex == listEventsCount)
                nextEventIndex = 0;

            _server.EventType = (GameEventType)nextEventIndex;
            await _context.SaveChangesAsync();

            _gameCycleController.OnChangeCycle((GameEventType)currentEventIndex, (GameEventType)nextEventIndex);
        }

        private async UniTaskVoid WaitTime(DelayType delayType, TimeSpan waitTime, Func<Task> onFinishWait, CancellationToken cancellationToken)
        {
            GameLogging.WriteGameLog($"Start wait time: {delayType}");
            var recordTime = GetRecordTime(delayType);
            var dateTime = DateTime.ParseExact(
                recordTime,
                Constants.Common.DateTimeFormat,
                CultureInfo.InvariantCulture
                );

            var now = DateTime.UtcNow;
            if (dateTime > now)
            {
                var delay = (dateTime - now).TotalMilliseconds;
                await Task.Delay((int)delay, cancellationToken);
            }

            await onFinishWait();

            var nextTime = DateTime.UtcNow.Add(waitTime);
            var extraTime = new TimeSpan(0, nextTime.Hour, nextTime.Minute, nextTime.Second);
            nextTime = nextTime.Subtract(extraTime);
            recordTime = nextTime.ToString(Constants.Common.DateTimeFormat);


            SetRecordTime(delayType, recordTime);
            await _context.SaveChangesAsync();
            WaitTime(delayType, waitTime, onFinishWait, cancellationToken).Forget();
        }

        [HttpPost]
        [Route("ServerController/FinishDay")]
        public async Task FinishDay()
        {
            await OnChangeDay();
        }

        [HttpPost]
        [Route("ServerController/FinishGameCycle")]
        public async Task FinishGameCycle()
        {
            await OnChangeGameCycle();
        }

        private string GetRecordTime(DelayType delayType)
        {
            var result = string.Empty;
            switch (delayType)
            {
                case DelayType.Day:
                    result = _server.NextDay;
                    break;
                case DelayType.Month:
                    result = _server.NextMonth;
                    break;
                case DelayType.Week:
                    result = _server.NextWeek;
                    break;
                case DelayType.GameCycle:
                    result = _server.NexGameCycle;
                    break;
            }
            return result;
        }

        private void SetRecordTime(DelayType delayType, string value)
        {
            GameLogging.WriteGameLog($"delayType: {delayType}, value: {value}");
            switch (delayType)
            {
                case DelayType.Day:
                    _server.NextDay = value;
                    break;
                case DelayType.Month:
                    _server.NextMonth = value;
                    break;
                case DelayType.Week:
                    _server.NextWeek = value;
                    break;
                case DelayType.GameCycle:
                    _server.NexGameCycle = value;
                    break;
            }
            GameLogging.WriteGameLog($"switch time, type{delayType}");
        }

        public void Dispose()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
            }
        }
    }
}
