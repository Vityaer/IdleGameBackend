namespace UniverseRift.Models.Teams
{
	public class ServerPlayerTeamData
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int PlayerId { get; set; }
		public string ArmyData { get; set; }

		public ServerPlayerTeamData()
		{
		}

		public ServerPlayerTeamData(int playerId, string name, string armyData)
		{
			PlayerId = playerId;
			Name = name;
			ArmyData = armyData;
		}
	}
}
