using UniverseRift.Controllers.Common;
using UniverseRift.GameModels;

namespace UniverseRift.GameModelDatas.Cities.Industries
{
    public class MineMissionData
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public string StorageMissionContainerId { get; set; }
        public string MissionId { get; set; } = string.Empty;
        public string UnitsStateJSON { get; set; } = string.Empty;
        public string DateTimeCreate { get; set; } = string.Empty;
        public bool IsComplete { get; set; }

        public MineMissionData()
        {
        }

        public MineMissionData(int playerId, string storageMissionContainerId, MissionModel missionModel)
        {
            PlayerId = playerId;
            IsComplete = false;
            StorageMissionContainerId = storageMissionContainerId;
            MissionId = missionModel.Name;
            DateTimeCreate = DateTime.UtcNow.ToString(Constants.Common.DateTimeFormat);
        }
    }
}
