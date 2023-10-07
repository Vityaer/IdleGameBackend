using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniverseRift.Contexts;
using UniverseRift.GameModelDatas.Cities.Industries;

namespace UniverseRift.Controllers.Buildings.Industries
{
    public class IndustryController : ControllerBase, IIndustryController
    {
        private readonly AplicationContext _context;

        public IndustryController(AplicationContext context)
        {
            _context = context;
        }

        public async Task<IndustryData> GetPlayerSave(int playerId, bool flagCreateNewData)
        {
            var result = new IndustryData();
            var mineDatas = await _context.MineDatas.ToListAsync();
            var playerMineDatas = mineDatas.FindAll(data => data.PlayerId == playerId);
            result.Mines.AddRange(playerMineDatas);
            return result;
        }
    }
}
