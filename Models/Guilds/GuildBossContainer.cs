using UniverseRift.Controllers.Common;
using UniverseRift.GameModels;

namespace UniverseRift.Models.Guilds
{
    public class GuildBossContainer : BaseModel
    {
        public List<GuildBossMission> Missions = new();
    }
}
