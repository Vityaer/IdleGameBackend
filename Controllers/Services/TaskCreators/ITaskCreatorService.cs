using UniverseRift.Models.Tasks.SimpleTask;

namespace UniverseRift.Controllers.Services.TaskCreators
{
	public interface ITaskCreatorService
	{
		void GetNewTasks(int playerId, int count, GameTaskSourceType gameTaskSourceType, out List<GameTask> resultTasks);
	}
}
