using Microsoft.AspNetCore.Mvc;
using UniverseRift.GameModelDatas.Cities;

namespace UniverseRift.Controllers.Buildings.Guilds
{
    public class GuildController : ControllerBase, IGuildController
    {
        public async Task<BuildingWithFightTeamsData> GetPlayerSave(int playerId)
        {
            var result = new BuildingWithFightTeamsData();
            return result;
        }
    }
}
