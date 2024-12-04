using UniverseRift.Models.Events;

namespace UniverseRift.Models.Common.Server
{
    public class ServerLifeTime
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string NextDay { get; set; } = string.Empty;
        public string NexGameCycle { get; set; } = string.Empty;
        public string NextWeek { get; set; } = string.Empty;
        public string NextMonth { get; set; } = string.Empty;
        public string LastStartDateTime { get; set; } = string.Empty;
        public GameEventType EventType { get; set; } = GameEventType.Fortune;

        public ServerLifeTime()
        {
            EventType = GameEventType.Fortune;
        }
    }
}
