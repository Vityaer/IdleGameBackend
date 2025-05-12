using Models.Data.Inventories;
using System;
using UniverseRift.GameModels.Common;

namespace UniverseRift.GameModels
{
    public class RewardModel : BaseModel
    {
        public List<ResourceData> Resources = new();
        public List<ItemData> Items = new();
        public List<SplinterData> Splinters = new();

        public void Add<T>(T subject) where T : InventoryBaseItem
        {
            switch(subject)
            {
                case ResourceData resource:
                    var oldResource = Resources.Find(res => res.Type.Equals(resource.Type));
                    if (oldResource != null)
                    {
                        oldResource.Amount.Add(resource.Amount);
                    }
                    else
                    {
                        var newResource = new ResourceData();
                        newResource.Type = resource.Type;
						newResource.Amount = new BigDigit(resource.Amount.Mantissa, resource.Amount.E10);
						Resources.Add(newResource);
                    }
                    break;
                case ItemData item:
                    Items.Add(item);
                    break;
                case SplinterData splinter:
                    Splinters.Add(splinter);
                    break;
            }
        }

		public static RewardModel operator *(RewardModel a, float factor)
		{
			RewardModel result = new RewardModel();
			foreach (ResourceData res in a.Resources)
			{
				result.Resources.Add(new ResourceData() { Type = res.Type, Amount = res.Amount * factor });
			}

			foreach (ItemData itemData in a.Items)
			{
				result.Items.Add(new ItemData() { Id = itemData.Id, Amount = itemData.Amount });
			}

			foreach (SplinterData splinter in a.Splinters)
			{
				int count = (int) Math.Clamp(Math.Floor(splinter.Amount * factor), 1, 100);
				result.Splinters.Add(new SplinterData() { Id = splinter.Id, Amount = count });
			}

			return result;
		}
	}
}