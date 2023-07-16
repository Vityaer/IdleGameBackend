using UniverseRift.Models.Resources;

namespace UniverseRift.GameModels
{
    public class GameResource
    {
        public readonly ResourceType Type;
        public readonly BigDigit Amount;

        public GameResource()
        {
            Type = ResourceType.Gold;
            Amount = new BigDigit();
        }

        public GameResource(ResourceData resourceModel)
        {
            Type = resourceModel.Type;
            Amount = resourceModel.Amount;
        }

        public GameResource(ResourceType name, float count = 0f, int e10 = 0)
        {
            Type = name;
            Amount = new BigDigit(count, e10);
        }

        public GameResource(ResourceType name, BigDigit amount)
        {
            Type = name;
            Amount = amount;
        }

        public void Add(int count)
        {
            AddResource(count);
        }

        public void AddResource(float count, float e10 = 0)
        {
            Amount.Add(count, e10);
        }

        public void AddResource(GameResource res)
        {
            Amount.Add(res.Amount);
        }

        public void SubtractResource(float count, float e10 = 0)
        {
            Amount.Subtract(count, e10);
        }

        public void SubtractResource(GameResource res)
        {
            Amount.Subtract(res.Amount);
        }

        public static GameResource operator *(GameResource res, float k)
        {
            GameResource result = new GameResource(res.Type, (float)Math.Ceiling(res.Amount.Mantissa * k), res.Amount.E10);
            return result;
        }
    }
}