using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Misc.Json;
using Models.Data.Inventories;
using Newtonsoft.Json;
using System.Reflection;
using UniverseRift.Contexts;
using UniverseRift.GameModels;
using UniverseRift.GameModels.Common;
using UniverseRift.Models.Heroes;
using UniverseRift.Models.Resources;
using UniverseRift.Models.Results;
using UniverseRift.Services.Resources;
using UniverseRift.Services.Rewarders;

namespace UniverseRift.Controllers.Buildings
{
	public class AltarController : ControllerBase
	{
		private readonly AplicationContext _context;
		private readonly IJsonConverter _jsonConverter;
		private readonly IResourceManager _resourcesController;
		private readonly IRewardService _clientRewardService;

		public AltarController(
			AplicationContext context,
			IJsonConverter jsonConverter,
			IResourceManager resourcesController,
			IRewardService clientRewardService
			)
		{
			_jsonConverter = jsonConverter;
			_context = context;
			_resourcesController = resourcesController;
			_clientRewardService = clientRewardService;
		}

		[HttpPost]
		[Route("Altar/RemoveHeroes")]
		public async Task<AnswerModel> RemoveHeroes(int playerId, string jsonContainer)
		{
			var answer = new AnswerModel();
			var fireContainer = JsonConvert.DeserializeObject<FireContainer>(jsonContainer);

			if (fireContainer == null)
			{
				answer.Error = "Wrong data";
				return answer;
			}

			var heroIds = fireContainer.HeroesIds;
			var heroCount = heroIds.Count;

			if (heroIds.Count == 0)
			{
				answer.Error = $"data error, hero count: {heroIds.Count}";
				return answer;
			}

			var heroes = await _context.Heroes.ToListAsync();
			int countReward = 0;
			foreach (var id in heroIds)
			{
				var hero = heroes.Find(hero => (hero.PlayerId == playerId) && (hero.Id == id));

				if (hero != null)
				{
					countReward += hero.Rating;
					_context.Heroes.Remove(hero);
				}
			}

			var reward = new RewardModel();
			var resource = new ResourceData { Type = ResourceType.RedDust, Amount = new BigDigit(10 * countReward) };
			reward.Add(resource);
			await _clientRewardService.AddReward(playerId, reward);

			await _context.SaveChangesAsync();

			answer.Result = _jsonConverter.Serialize(reward);
			return answer;
		}
	}
}
