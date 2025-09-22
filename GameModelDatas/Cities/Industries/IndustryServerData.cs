using UniverseRift.Controllers.Common;

namespace UniverseRift.GameModelDatas.Cities.Industries
{
	public class IndustryServerData
	{
		public int Id { get; set; }
		public int PlayerId { get; set; }
		public int MineEnergy { get; set; }
		public string DateTimeCreate { get; set; } = string.Empty;

		public IndustryServerData()
		{
			DateTimeCreate = DateTime.UtcNow.ToString(Constants.Common.DateTimeFormat);
		}
	}
}
