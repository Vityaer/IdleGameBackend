using Microsoft.AspNetCore.Mvc;
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

        public async Task<IndustryModel> GetPlayerSave(int playerId)
        {
            var result = new IndustryModel();
            return result;
        }
    }
}
