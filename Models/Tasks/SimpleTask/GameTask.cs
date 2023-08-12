using UniverseRift.Models.Common;

namespace UniverseRift.Models.Tasks.SimpleTask
{
    public class GameTask
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public string TaskModelId { get; set; }
        public TaskStatusType Status { get; set; } = TaskStatusType.NotStart;
        public string DateTimeCreate { get; set; } = string.Empty;
        public string DateTimeStart { get; set; } = string.Empty;
        public float RewardFactor { get; set; }

        public GameTask() { }

        public GameTask(int playerId, GameTaskModel taskModel, float randFactor)
        {
            DateTimeCreate = DateTime.UtcNow.ToString();
            PlayerId = playerId;
            TaskModelId = taskModel.Id;
            Status = TaskStatusType.NotStart;
            DateTimeStart = string.Empty;
            RewardFactor = randFactor;
        }

        public void Start()
        {
            Status = TaskStatusType.InProgress;
            DateTimeStart = DateTime.UtcNow.ToString();
        }

        public bool CheckComplete(int hours)
        {
            if (Status == TaskStatusType.Completed)
            {
                return true;
            }

            var now = DateTime.UtcNow;
            var dateTimeStart = DateTime.Parse(DateTimeStart);
            var delta = now - dateTimeStart;
            if (delta.TotalHours > hours)
            {
                Status = TaskStatusType.Completed;
                return true;
            }

            return false;
        }
    }
}
