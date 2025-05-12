using UniverseRift.Models.Arenas;

namespace UniverseRift.Controllers.Buildings.Arenas
{
	public class ArenaFighterComparer : IComparer<ServerArenaPlayerData>
	{
		public int Compare(ServerArenaPlayerData? x, ServerArenaPlayerData? y)
		{
			if (x.Score > y.Score)
			{
				return 1;
			}
			else if (x.Score < y.Score)
			{
				return -1;
			}
			else
			{
				if (x.MaxScore > y.MaxScore)
				{
					return 1;
				}
				else if (x.MaxScore < y.MaxScore)
				{
					return -1;
				}
				else
				{
					return 0;
				}
			}
		}
	}
}
