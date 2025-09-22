namespace UniverseRift.Models.Guilds
{
	public class GuildRecruitDamageComparer : IComparer<RecruitData>
	{
		public int Compare(RecruitData x, RecruitData y)
		{
			if (x.ResultE10 > y.ResultE10)
			{
				return -1;
			}
			else if (x.ResultE10 < y.ResultE10)
			{
				return 1;
			}
			else
			{
				if (x.ResultMantissa > y.ResultMantissa)
				{
					return -1;
				}
				else if (x.ResultMantissa < y.ResultMantissa)
				{
					return 1;
				}
				else
				{
					return 0;
				}
			}
		}
	}
}
