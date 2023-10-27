using UniverseRift.GameModelDatas;

namespace UniverseRift.Models.Achievments
{
    public class AchievmentData : BaseDataModel
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public string ModelId { get; set; } = string.Empty;

        public int CurrentStage;
        public float Amount;
        public int E10;
        public bool IsComplete;

        public AchievmentData(int playerId, string modelId)
        {
            PlayerId = playerId;
            ModelId = modelId;
            CurrentStage = 0;
            Amount = 0;
            E10 = 0;
            IsComplete = false;
        }
    }
}
