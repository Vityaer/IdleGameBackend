namespace UniverseRift.Models.Arenas
{
	public class ArenaSeason
	{
		public int Id { get; set; }
		public ArenaType ArenaType { get; set; }
		public string StartDateTime { get; set; }

		public ArenaSeason() { }

		public ArenaSeason(ArenaType arenaType, string startDateTime)
		{
			ArenaType = arenaType;
			StartDateTime = startDateTime;
		}
	}
}
