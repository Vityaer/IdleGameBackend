using UniverseRift.GameModels;
using UniverseRift.Models.Common;

namespace UniverseRift.Models.Resources
{
    public class Resource : BaseInventoryObject
    {
        public Resource()
        {
        }

        public Resource(int playerId, GameResource resource)
        {
            PlayerId = playerId;
            Type = resource.Type;
            Count = resource.Amount.Mantissa;
            E10 = resource.Amount.E10;
        }

        public ResourceType Type { get; set; }

        public void Add(Resource newRes)
        {
            Count += newRes.Count * (float)Math.Pow(10f, newRes.E10 - E10);
            NormalizeDigit();
        }

        public void Subtract(Resource newRes)
        {
            Count -= Count * (float)Math.Pow(10f, newRes.E10 - E10);
            NormalizeDigit();
        }

        public bool CheckCount(float count, int e10)
        {
            var result = false;
            if (E10 != e10)
            {
                if (E10 > e10)
                {
                    if (Count * (float)Math.Pow(10, E10 - e10) >= count)
                        result = true;
                }
                else
                {
                    if (Count >= count * (float)Math.Pow(10, e10 - E10))
                        result = true;
                }
            }
            else
            {
                if (Count >= count)
                {
                    result = true;
                }
            }
            return result;
        }

        public void NormalizeDigit()
        {
            while (Count > 1000)
            {
                E10 += 3;
                Count *= 0.001f;
            }

            while (Count < 1 && E10 > 0)
            {
                E10 -= 3;
                Count *= 1000f;
            }

            if (E10 == 0)
            {
                Count = (float)Math.Round(Count);
            }
            else
            {
                Count = (float)Math.Round(Count * 1000f) * 0.001f;
            }
        }

        public static Resource operator *(Resource res, float k)
        {
            res.Count = res.Count * k;
            res.NormalizeDigit();
            return res;
        }
    }
}
