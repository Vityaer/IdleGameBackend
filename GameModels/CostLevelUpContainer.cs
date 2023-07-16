using System;
using UniverseRift.Models.Resources;

namespace UniverseRift.GameModels
{
    public class CostLevelUpContainer : BaseModel
    {
        public List<CostLevelUpModel> LevelsCost = new List<CostLevelUpModel>();

        public CostLevelUpContainer()
        {
        }

        public List<Resource> GetCostForLevelUp(int level, int playerId)
        {
            var result = new List<Resource>();

            var work = LevelsCost[0];

            for (int i = 0; i < LevelsCost.Count; i++)
            {
                if (LevelsCost[i].level == level)
                {
                    work = LevelsCost[i];
                    break;
                }
                if (LevelsCost[i].level < level)
                    work = LevelsCost[i];
            }

            GameResource res = null;
            for (int i = 0; i < work.Cost.Count; i++)
            {
                var data = work.Cost[i];
                res = new GameResource(data.Type, data.Amount);

                switch (work.typeIncrease)
                {
                    case CostIncreaseType.Mulitiply:
                        res = res * (float)Math.Pow(1 + work.ListIncrease[i] / 100f, level - work.level);
                        break;
                    case CostIncreaseType.Add:
                        res.AddResource(work.ListIncrease[i]);
                        break;
                }
                result.Add(new Resource {Type = res.Type, Count = res.Amount.Mantissa, E10 = res.Amount.E10, PlayerId = playerId });
            }
            return result;
        }
    }
}