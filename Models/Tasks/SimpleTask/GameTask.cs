namespace UniverseRift.Models.Tasks.SimpleTask
{
    public class GameTask
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int Hours { get; set; }
        public TaskStatusType Status { get; set; } = TaskStatusType.NotStart;
        public string DateTimeStart { get; set; } = string.Empty; 

        public GameTask() { }

        public GameTask(int playerId, GameTaskTemplate template)
        {
        }

        public void Start()
        {
            Status = TaskStatusType.InProgress;
            DateTimeStart = DateTime.UtcNow.ToString();
        }

        public bool CheckComplete()
        {
            if (Status == TaskStatusType.Completed)
            {
                return true;
            }

            var now = DateTime.UtcNow;
            var dateTimeStart = DateTime.Parse(DateTimeStart);
            var delta = now - dateTimeStart;
            if (delta.TotalHours > Hours)
            {
                Status = TaskStatusType.Completed;
                return true;
            }

            return false;
        }
    }
}
