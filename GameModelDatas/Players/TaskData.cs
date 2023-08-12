using UniverseRift.Models.Tasks;
using UniverseRift.Models.Tasks.SimpleTask;

namespace UniverseRift.GameModelDatas.Players
{
    public class TaskData : BaseDataModel
    {
        public int TaskId;
        public string TaskModelId;
        public TaskStatusType Status;
        public string DateTimeStart;

        public TaskData(GameTask task)
        {
            TaskId = task.Id;
            TaskModelId = task.TaskModelId;
            Status = task.Status;
            DateTimeStart = task.DateTimeStart;
        }
    }
}