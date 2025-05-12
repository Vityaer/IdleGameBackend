using Cysharp.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Misc.Json;
using System.Globalization;
using System.Threading.Tasks;
using UniRx;
using UniverseRift.Contexts;
using UniverseRift.Controllers.Buildings.Achievments;
using UniverseRift.Controllers.Common;
using UniverseRift.Controllers.Server;
using UniverseRift.Controllers.Services.TaskCreators;
using UniverseRift.GameModelDatas.Players;
using UniverseRift.Heplers.Utils;
using UniverseRift.Models.City.Markets;
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
        private const int DAY_FOR_DELETE = 3;
        private const int DAILY_TASK_COUNT = 5;

        private readonly AplicationContext _context;
        private readonly IJsonConverter _jsonConverter;
        private readonly IResourceManager _resourceController;
        private readonly ICommonDictionaries _commonDictionaries;
        private readonly IRewardService _clientRewardService;
        private readonly IAchievmentController _achievmentController;
        private readonly ITaskCreatorService _taskCreatorService;

		private readonly Random _random = new Random();

        public TaskBoardController(
            AplicationContext context,
            IJsonConverter jsonConverter,
            IResourceManager resourceController,
            ICommonDictionaries commonDictionaries,
            IRewardService clientRewardService,
            IAchievmentController achievmentController,
			ITaskCreatorService taskCreatorService
			)
        {
            _commonDictionaries = commonDictionaries;
            _achievmentController = achievmentController;
            _context = context;
            _clientRewardService = clientRewardService;
            _jsonConverter = jsonConverter;
            _resourceController = resourceController;
			_taskCreatorService = taskCreatorService;

		}

		public async Task<TaskBoardData> GetPlayerSave(int playerId, bool flagCreateNewData)
		{
			var result = new TaskBoardData();
			var allTasks = await _context.GameTasks.ToListAsync();

			List<GameTask> playerTasks = new(10);

			var playerAllTasks = allTasks.FindAll(task => task.PlayerId == playerId);
			foreach (var task in playerAllTasks)
			{
				if (_commonDictionaries.GameTaskModels.TryGetValue(task.TaskModelId, out var taskModel))
				{
					if (taskModel.SourceType == GameTaskSourceType.Taskboard)
					{
						playerTasks.Add(task);
					}
				}
			}

			if (flagCreateNewData)
			{
				var notStartableTasks = playerTasks.FindAll(task => task.Status == TaskStatusType.NotStart);

				if (notStartableTasks.Count < DAILY_TASK_COUNT)
				{
					var notEnoughCount = DAILY_TASK_COUNT - notStartableTasks.Count;
					_taskCreatorService.GetNewTasks(playerId, notEnoughCount, GameTaskSourceType.Taskboard, out var newTasks);
					playerTasks.AddRange(newTasks);

					await _context.GameTasks.AddRangeAsync(newTasks);
					await _context.SaveChangesAsync();
				}
			}

			foreach (var task in playerTasks)
			{
				result.ListTasks.Add(new TaskData(task));
			}

			return result;
		}

		public async Task DeleteTasks()
        {
            var allTasks = await _context.GameTasks.ToListAsync();
            var now = DateTime.UtcNow;

            var tasksForDelete = new List<GameTask>();
            foreach (var task in allTasks)
            {
                var taskModel = _commonDictionaries.GameTaskModels[task.TaskModelId];
                task.CheckComplete(taskModel.RequireHour);

                if (task.Status != TaskStatusType.InProgress)
                {
                    var dateTimeCreate = DateTimeUtils.TryParseOrNow(task.DateTimeCreate);

                    var timeSpan = now - dateTimeCreate;
                    if (timeSpan.TotalDays > DAY_FOR_DELETE)
                    {
                        tasksForDelete.Add(task);
                    }
                }
            }
            await _context.SaveChangesAsync();

            if (tasksForDelete.Count > 0)
            {
                _context.GameTasks.RemoveRange(tasksForDelete);
                await _context.SaveChangesAsync();

            }
        }

        [HttpPost]
        [Route("TaskBoard/StartTask")]
        public async Task<AnswerModel> StartTask(int playerId, int taskId)
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
        public async Task<AnswerModel> CompleteTask(int playerId, int taskId)
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

            await _clientRewardService.AddReward(playerId, taskModel.Reward * task.RewardFactor);

            _context.GameTasks.Remove(task);
            await _context.SaveChangesAsync();

            await _achievmentController.AchievmentUpdataData(playerId, "CompleteTaskCountAchievment", 1);

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

            _taskCreatorService.GetNewTasks(playerId, 1, GameTaskSourceType.Taskboard, out var resultTasks);

            _context.GameTasks.AddRange(resultTasks);
            await _context.SaveChangesAsync();

            answer.Result = _jsonConverter.Serialize(resultTasks);
            return answer;
        }

        [HttpPost]
        [Route("TaskBoard/BuyFastCompleteTask")]
        public async Task<AnswerModel> BuyFastCompleteTask(int playerId, int taskId)
        {
            var answer = new AnswerModel();

            var task = await _context.GameTasks.FindAsync(taskId);

            var permission = CheckTask(task, playerId, answer);
            if (!permission)
            {
                return answer;
            }

            var taskModel = _commonDictionaries.GameTaskModels[task.TaskModelId];

            if (taskModel.SourceType != GameTaskSourceType.Taskboard)
            {
                answer.Error = "You can't speed up tasks from not Taskboard";
				return answer;
            }

            var cost = new Resource() { PlayerId = playerId, Type = ResourceType.Diamond, Count = 10 * taskModel.Rating, E10 = 0 };

            permission = await _resourceController.CheckResource(playerId, cost, answer);
            if (!permission)
            {
                return answer;
            }

            await _resourceController.SubstactResources(cost);

            await _clientRewardService.AddReward(playerId, taskModel.Reward * task.RewardFactor);
            _context.GameTasks.Remove(task);
            await _context.SaveChangesAsync();

            answer.Result = Constants.Common.SUCCESS_RUSULT;
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

			_taskCreatorService.GetNewTasks(playerId, 1, GameTaskSourceType.Taskboard, out var resultTasks);
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

			_taskCreatorService.GetNewTasks(playerId, playerNotStartedTasks.Count, GameTaskSourceType.Taskboard, out var resultTasks);

            _context.GameTasks.RemoveRange(playerNotStartedTasks);
            _context.GameTasks.AddRange(resultTasks);

            await _context.SaveChangesAsync();

            answer.Result = _jsonConverter.Serialize(resultTasks);
            return answer;
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

      
    }
}
