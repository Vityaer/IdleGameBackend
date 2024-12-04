using System.Globalization;
using UniverseRift.Controllers.Common;
using UniverseRift.Heplers.Utils;

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
            DateTimeCreate = DateTime.UtcNow.ToString(Constants.Common.DateTimeFormat);
            PlayerId = playerId;
            TaskModelId = taskModel.Id;
            Status = TaskStatusType.NotStart;
            DateTimeStart = string.Empty;
            RewardFactor = randFactor;
        }

        public void Start()
        {
            Status = TaskStatusType.InProgress;
            DateTimeStart = DateTime.UtcNow.ToString(Constants.Common.DateTimeFormat);
        }

        public bool CheckComplete(int hours)
        {
            if (Status == TaskStatusType.Completed)
            {
                return true;
            }

            if (string.IsNullOrEmpty(DateTimeStart))
                return false;


            var now = DateTime.UtcNow;
            var dateTimeStart = DateTimeUtils.TryParseOrNow(DateTimeStart);

            var delta = now - dateTimeStart;
            if (delta.TotalHours > hours)
            {
                return true;
            }

            return false;
        }
    }
}
