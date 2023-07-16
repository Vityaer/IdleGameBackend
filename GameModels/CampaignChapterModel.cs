namespace UniverseRift.GameModels
{
    public class CampaignChapterModel : BaseModel
    {
        public string Name;
        public int numChapter;
        public List<CampaignMissionModel> Missions = new List<CampaignMissionModel>();
    }
}
