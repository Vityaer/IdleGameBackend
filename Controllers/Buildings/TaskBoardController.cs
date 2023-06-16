using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Misc.Json;
using System.Threading.Tasks;
using UniverseRift.Contexts;
using UniverseRift.Controllers.Players.Inventories.Resources;
using UniverseRift.Models.Common;
using UniverseRift.Models.Resources;
using UniverseRift.Models.Results;
using UniverseRift.Models.Tasks;
using UniverseRift.Models.Tasks.SimpleTask;

namespace UniverseRift.Controllers.Buildings
{
    public class TaskBoardController : ControllerBase
    {
        private readonly AplicationContext _context;
        private readonly IJsonConverter _jsonConverter;
        private readonly IResourceController _resourceController;
        private readonly Random _random = new Random();

        public TaskBoardController(AplicationContext context, IJsonConverter jsonConverter, IResourceController resourceController)
        {
            _context = context;
            _jsonConverter = jsonConverter;
            _resourceController = resourceController;
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

            return answer;
        }

        [HttpPost]
        [Route("TaskBoard/StartTask")]
        public async Task<AnswerModel> CompleteTask(int taskId, int playerId)
        {
            var answer = new AnswerModel();

            var task = await _context.GameTasks.FindAsync(taskId);

            var permition = CheckTask(task, playerId, answer);
            if (!permition)
            {
                return answer;
            }

            var complete = task.CheckComplete();
            if (!complete)
            {
                answer.Error = "Task not complete yet";
                return answer;
            }

            _context.GameTasks.Remove(task);
            await _context.SaveChangesAsync();
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

            var resultTasks = await GetNewTasks(playerId);

            _context.GameTasks.AddRange(resultTasks);
            await _context.SaveChangesAsync();

            answer.Result = _jsonConverter.ToJson(resultTasks);
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

            var resultTasks = await GetNewTasks(playerId);
            _context.GameTasks.AddRange(resultTasks);
            await _context.SaveChangesAsync();

            answer.Result = _jsonConverter.ToJson(resultTasks);
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

            var resultTasks = await GetNewTasks(playerId);

            _context.GameTasks.RemoveRange(playerNotStartedTasks);
            _context.GameTasks.AddRange(resultTasks);

            await _context.SaveChangesAsync();

            answer.Result = _jsonConverter.ToJson(resultTasks);
            return answer;
        }

        private async Task<List<GameTask>> GetNewTasks(int playerId, int count = 1)
        {
            var resultTasks = new List<GameTask>(count);
            var gameTaskTemplates = await _context.GameTaskTemplates.ToListAsync();

            for (var i = 0; i < count; i++)
            {
                var randomTemplate = gameTaskTemplates[_random.Next(0, gameTaskTemplates.Count)];
                var newTask = new GameTask(playerId, randomTemplate);
                resultTasks.Add(newTask);
            }

            return resultTasks;
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
