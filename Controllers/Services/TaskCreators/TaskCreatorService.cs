using UniverseRift.Controllers.Common;
using UniverseRift.Models.Tasks.SimpleTask;

namespace UniverseRift.Controllers.Services.TaskCreators
{
    public class TaskCreatorService : ITaskCreatorService
	{
		private readonly static Random _random = new Random();
		private readonly ICommonDictionaries m_commonDictionaries;

		public TaskCreatorService(ICommonDictionaries commonDictionaries)
		{
			m_commonDictionaries = commonDictionaries;
		}

		public void GetNewTasks(int playerId, int count, GameTaskSourceType gameTaskSourceType, out List<GameTask> resultTasks)
		{
			var gameTaskModels = m_commonDictionaries.GameTaskModels
				.Where(task => task.Value.SourceType == gameTaskSourceType)
				.Select(task => task.Value)
				.ToList();

			resultTasks = new List<GameTask>(count);

			for (var i = 0; i < count; i++)
			{
				var random = _random.Next(0, gameTaskModels.Count);
				var taskModel = gameTaskModels[random];

				var intFactorDelta = (int)(taskModel.FactorDelta * 100f);
				var randFactor = _random.Next(100 - intFactorDelta, 100 + intFactorDelta + 1) / 100f;
				randFactor = (float)Math.Round(randFactor, 2);

				var newTask = new GameTask(playerId, taskModel, randFactor);
				resultTasks.Add(newTask);
			}
		}
	}
}
