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

        public readonly ReactiveCommand OnChangeDay = new ReactiveCommand();
        public readonly ReactiveCommand OnChangeWeek = new ReactiveCommand();
        public readonly ReactiveCommand OnChangeMonth = new ReactiveCommand();
        public readonly ReactiveCommand OnChangeGameCycle = new ReactiveCommand();

        private readonly TimeSpan Day = new TimeSpan(24, 0, 0);
        private readonly TimeSpan Week = new TimeSpan(7, 0, 0, 0);
        private readonly TimeSpan Month = new TimeSpan(30, 0, 0, 0);
        private readonly TimeSpan GameCycle = new TimeSpan(5, 0, 0, 0);

        private ServerLifeTime _server;
        private CancellationTokenSource _cancellationTokenSource;

        public ServerController(AplicationContext context)
        {
            _context = context;
            _cancellationTokenSource = new CancellationTokenSource();
            Start(_cancellationTokenSource.Token).Forget();
        }

        public async UniTaskVoid Start(CancellationToken cancellationToken)
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

            WaitTime(_server.NextDay, Day, OnChangeDay, cancellationToken).Forget();
            WaitTime(_server.NextWeek, Week, OnChangeWeek, cancellationToken).Forget();
            WaitTime(_server.NextMonth, Month, OnChangeMonth, cancellationToken).Forget();
            WaitTime(_server.NexGameCycle, GameCycle, OnChangeGameCycle, cancellationToken).Forget();

            await _context.SaveChangesAsync();
        }

        private async UniTaskVoid WaitTime(string recordTime, TimeSpan nextTime, ReactiveCommand onFinishWait, CancellationToken cancellationToken)
        {
            var dateTime = DateTime.Parse(recordTime);
            var now = DateTime.UtcNow;
            var delay = (dateTime - now).TotalMilliseconds;
            await Task.Delay((int) delay, cancellationToken);

            onFinishWait.Execute();
            
            recordTime = DateTime.Now.Add(nextTime).ToString();
            await _context.SaveChangesAsync();
            WaitTime(recordTime, nextTime, onFinishWait, cancellationToken).Forget();

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
