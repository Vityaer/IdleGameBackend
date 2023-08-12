using Cysharp.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Misc.Json;
using UniRx;
using UniverseRift.Contexts;
using UniverseRift.Controllers.Common;
using UniverseRift.Controllers.Server;
using UniverseRift.GameModelDatas.Players;
using UniverseRift.Models.City.TaskBoards;
using UniverseRift.Models.Resources;
using UniverseRift.Models.Results;
using UniverseRift.Models.Tasks;
using UniverseRift.Models.Tasks.SimpleTask;
using UniverseRift.Services.Resources;
using UniverseRift.Services.Rewarders;

namespace UniverseRift.Controllers.Buildings.TaskBoards
{
    public class TaskBoardController : ControllerBase, ITaskBoardController
    {
        private const int DAY_FOR_DELETE = 10;
        private const int DAILY_TASK_COUNT = 5;

        private readonly AplicationContext _context;
        private readonly IJsonConverter _jsonConverter;
        private readonly IResourceManager _resourceController;
        private readonly ICommonDictionaries _commonDictionaries;
        private readonly IServerController _serverController;
        private readonly IRewardService _clientRewardService;

        private readonly Random _random = new Random();
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public TaskBoardController(
            AplicationContext context,
            IJsonConverter jsonConverter,
            IResourceManager resourceController,
            IServerController serverController,
            ICommonDictionaries commonDictionaries,
            IRewardService clientRewardService
            )
        {
            _commonDictionaries = commonDictionaries;
            _context = context;
            _clientRewardService = clientRewardService;
            _jsonConverter = jsonConverter;
            _resourceController = resourceController;
            _serverController = serverController;
            _serverController.OnChangeDay.Subscribe(_ => DeleteTasks().Forget()).AddTo(_disposables);
        }

        private async UniTaskVoid DeleteTasks()
        {
            var allTasks = await _context.GameTasks.ToListAsync();
            var now = DateTime.UtcNow;

            foreach (var task in allTasks)
            {
                var taskModel = _commonDictionaries.GameTaskModels[task.TaskModelId];
                task.CheckComplete(taskModel.RequireHour);

                if (task.Status != TaskStatusType.InProgress)
                {
                    var dateTimeCreate = DateTime.Parse(task.DateTimeCreate);
                    var timeSpan = now - dateTimeCreate;
                    if (timeSpan.TotalDays > DAY_FOR_DELETE)
                    {
                        _context.GameTasks.Remove(task);
                    }
                }
            }
            await _context.SaveChangesAsync();
        }

        [HttpPost]
        [Route("TaskBoard/StartTask")]
        public async Task<AnswerModel> StartTask(int taskId, int playerId)
        {
            var answer = new AnswerModel();

            var task = await _context.GameTasks.FindAsync(taskId);

            var permition = CheckTask(task, playerId, answer);
            if (!permition)
            {
                return answer;
            }

            if (task.Status != TaskStatusType.NotStart)
            {
                answer.Error = "Task already in progress";
                return answer;
            }

            task.Start();
            await _context.SaveChangesAsync();

            answer.Result = "Success";
            return answer;
        }

        [HttpPost]
        [Route("TaskBoard/CompleteTask")]
        public async Task<AnswerModel> CompleteTask(int taskId, int playerId)
        {
            var answer = new AnswerModel();

            var task = await _context.GameTasks.FindAsync(taskId);

            var permition = CheckTask(task, playerId, answer);
            if (!permition)
            {
                return answer;
            }

            var taskModel = _commonDictionaries.GameTaskModels[task.TaskModelId];
            var complete = task.CheckComplete(taskModel.RequireHour);
            if (!complete)
            {
                answer.Error = "Task not complete yet";
                return answer;
            }

            await _clientRewardService.AddReward(playerId, taskModel.Reward);

            _context.GameTasks.Remove(task);
            await _context.SaveChangesAsync();

            answer.Result = "Success";
            return answer;
        }

        [HttpPost]
        [Route("TaskBoard/BuySimpleTask")]
        public async Task<AnswerModel> BuySimpleTask(int playerId)
        {
            var answer = new AnswerModel();

            var cost = new Resource() { PlayerId = playerId, Type = ResourceType.SimpleTask, Count = 1, E10 = 0 };

            var permission = await _resourceController.CheckResource(playerId, cost, answer);
            if (!permission)
            {
                return answer;
            }

            await _resourceController.SubstactResources(cost);

            GetNewTasks(playerId, 1, out var resultTasks);

            _context.GameTasks.AddRange(resultTasks);
            await _context.SaveChangesAsync();

            answer.Result = _jsonConverter.Serialize(resultTasks);
            return answer;
        }

        [HttpPost]
        [Route("TaskBoard/BuySpecialTask")]
        public async Task<AnswerModel> BuySpecialTask(int playerId)
        {
            var answer = new AnswerModel();

            var cost = new Resource() { PlayerId = playerId, Type = ResourceType.SpecialTask, Count = 1, E10 = 0 };

            var permission = await _resourceController.CheckResource(playerId, cost, answer);
            if (!permission)
            {
                return answer;
            }

            await _resourceController.SubstactResources(cost);

            GetNewTasks(playerId, 1, out var resultTasks);
            _context.GameTasks.AddRange(resultTasks);
            await _context.SaveChangesAsync();

            answer.Result = _jsonConverter.Serialize(resultTasks);
            return answer;
        }


        [HttpPost]
        [Route("TaskBoard/ReplaceTasks")]
        public async Task<AnswerModel> ReplaceTasks(int playerId)
        {
            var answer = new AnswerModel();

            var allTasks = await _context.GameTasks.ToListAsync();

            var playerNotStartedTasks = allTasks.FindAll(task => task.PlayerId == playerId && task.Status == TaskStatusType.NotStart);
            var cost = new Resource() { PlayerId = playerId, Type = ResourceType.Diamond, Count = 10, E10 = 0 } * playerNotStartedTasks.Count;

            var permission = await _resourceController.CheckResource(playerId, cost, answer);
            if (!permission)
            {
                return answer;
            }

            await _resourceController.SubstactResources(cost);

            GetNewTasks(playerId, 1, out var resultTasks);

            _context.GameTasks.RemoveRange(playerNotStartedTasks);
            _context.GameTasks.AddRange(resultTasks);

            await _context.SaveChangesAsync();

            answer.Result = _jsonConverter.Serialize(resultTasks);
            return answer;
        }

        private void GetNewTasks(int playerId, int count, out List<GameTask> resultTasks)
        {
            var gameTaskModels = _commonDictionaries.GameTaskModels.ToList();

            resultTasks = new List<GameTask>(count);

            for (var i = 0; i < count; i++)
            {
                var random = _random.Next(0, gameTaskModels.Count);
                var taskModel = gameTaskModels[random].Value;

                var intFactorDelta = (int)(taskModel.FactorDelta * 100f);
                var randFactor = _random.Next(100 - intFactorDelta, 100 + intFactorDelta + 1) / 100f;
                randFactor = (float)Math.Round(randFactor, 2);

                var newTask = new GameTask(playerId, taskModel, randFactor);
                resultTasks.Add(newTask);
            }
        }

        private bool CheckTask(GameTask task, int playerId, AnswerModel answer)
        {
            if (task == null)
            {
                answer.Error = "Task not found";
                return false;
            }

            if (task.PlayerId != playerId)
            {
                answer.Error = "Data error";
                return false;
            }

            return true;
        }

        public async Task<TaskBoardData> GetPlayerSave(int playerId)
        {
            var result = new TaskBoardData();
            var allTasks = await _context.GameTasks.ToListAsync();

            var playerTasks = allTasks.FindAll(task => task.PlayerId == playerId);

            var notStartableTasks = playerTasks.FindAll(task => task.Status == TaskStatusType.NotStart);

            if (notStartableTasks.Count < DAILY_TASK_COUNT)
            {
                var notEnoughCount = DAILY_TASK_COUNT - notStartableTasks.Count;
                GetNewTasks(playerId, notEnoughCount, out var newTasks);
                playerTasks.AddRange(newTasks);
            }

            foreach (var task in playerTasks)
            {
                result.ListTasks.Add(new TaskData(task));
            }

            return result;
        }
    }
}
