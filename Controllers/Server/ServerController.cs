using Cysharp.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniRx;
using UniverseRift.Contexts;
using UniverseRift.Models.Common.Server;

namespace UniverseRift.Controllers.Server
{
    public class ServerController : ControllerBase, IServerController, IDisposable
    {
        private readonly AplicationContext _context;

        public readonly ReactiveCommand _onChangeDay = new ReactiveCommand();
        public readonly ReactiveCommand _onChangeWeek = new ReactiveCommand();
        public readonly ReactiveCommand _onChangeMonth = new ReactiveCommand();
        public readonly ReactiveCommand _onChangeGameCycle = new ReactiveCommand();

        private readonly TimeSpan Day = new TimeSpan(24, 0, 0);
        private readonly TimeSpan Week = new TimeSpan(7, 0, 0, 0);
        private readonly TimeSpan Month = new TimeSpan(30, 0, 0, 0);
        private readonly TimeSpan GameCycle = new TimeSpan(5, 0, 0, 0);

        private ServerLifeTime _server;
        private CancellationTokenSource _cancellationTokenSource;

        public ReactiveCommand OnChangeDay => _onChangeDay;
        public ReactiveCommand OnChangeWeek => _onChangeWeek;
        public ReactiveCommand OnChangeMonth => _onChangeMonth;
        public ReactiveCommand OnChangeGameCycle => _onChangeGameCycle;


        public ServerController(AplicationContext context)
        {
            _context = context;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void OnStartProject()
        {
            Start(_cancellationTokenSource.Token).Forget();
        }

        private async UniTaskVoid Start(CancellationToken cancellationToken)
        {
            var servers = await _context.ServerLifeTimes.ToListAsync();

            if (servers.Count == 0)
            {
                _server = new ServerLifeTime() { Id = 1 };
                var now = DateTime.UtcNow;
                _server.LastStartDateTime = now.ToString();

                var extraTime = new TimeSpan(0, now.Hour, now.Minute, now.Second);
                var startCurrentDay = now.Subtract(extraTime);
                _server.NextDay = startCurrentDay.Add(Day).ToString();
                _server.NextWeek = startCurrentDay.Add(Week).ToString();
                _server.NextMonth = startCurrentDay.Add(Month).ToString();
                _server.NexGameCycle = startCurrentDay.Add(GameCycle).ToString();

                await _context.ServerLifeTimes.AddAsync(_server);

            }
            else
            {
                _server = servers[0];
            }
            _onChangeDay.Execute();
            WaitTime(DelayType.Day, Day, OnChangeDay, cancellationToken).Forget();
            WaitTime(DelayType.Week, Week, OnChangeWeek, cancellationToken).Forget();
            WaitTime(DelayType.Month, Month, OnChangeMonth, cancellationToken).Forget();
            WaitTime(DelayType.GameCycle, GameCycle, OnChangeGameCycle, cancellationToken).Forget();

            await _context.SaveChangesAsync();
        }

        private async UniTaskVoid WaitTime(DelayType delayType, TimeSpan waitTime, ReactiveCommand onFinishWait, CancellationToken cancellationToken)
        {
            var recordTime = GetRecordTime(delayType);
            var dateTime = DateTime.Parse(recordTime);
            var now = DateTime.UtcNow;
            if (dateTime > now)
            {
                var delay = (dateTime - now).TotalMilliseconds;
                await Task.Delay((int) delay, cancellationToken);
            }

            onFinishWait.Execute();

            var nextTime = DateTime.Now.Add(waitTime);
            var extraTime = new TimeSpan(0, nextTime.Hour, nextTime.Minute, nextTime.Second);
            nextTime = nextTime.Subtract(extraTime);
            recordTime = nextTime.ToString();

            SetRecordTime(delayType, recordTime);
            await _context.SaveChangesAsync();
            WaitTime(delayType, waitTime, onFinishWait, cancellationToken).Forget();

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
        }

        public void Dispose()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
            }
        }

        public void StopApplication()
        {
            throw new NotImplementedException();
        }
    }
}
