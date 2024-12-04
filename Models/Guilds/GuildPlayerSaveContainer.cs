namespace UniverseRift.Models.Guilds
{
    [Serializable]
    public class GuildPlayerSaveContainer
    {
        public GuildData GuildData;
        public List<RecruitData> GuildRecruits = new();
        public List<GuildPlayerRequest> Requests = new();

        public GuildPlayerSaveContainer()
        {
        }

        public GuildPlayerSaveContainer(GuildData guildData, List<RecruitData> guildRecruits)
        {
            GuildData = guildData;
            GuildRecruits = guildRecruits;
        }
    }
}
