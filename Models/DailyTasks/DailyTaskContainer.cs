namespace UniverseRift.Models.DailyTasks
{
    public class DailyTaskContainer
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public string DailyTasksJson { get; set; } = string.Empty;
    }
}
