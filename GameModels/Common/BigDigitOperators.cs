namespace UniverseRift.GameModels.Common
{
    public partial class BigDigit
    {
        public static BigDigit operator *(BigDigit res, float k)
        {
            BigDigit result = new BigDigit(res.Mantissa * k, res.E10);
            result.NormalizeDigit();
            return result;
        }

        public static BigDigit operator *(BigDigit res, int k)
        {
            BigDigit result = new BigDigit(res.Mantissa * k, res.E10);
            result.NormalizeDigit();
            return result;
        }

        public static BigDigit operator /(BigDigit a, BigDigit b)
        {
            BigDigit result = new BigDigit(0, 0);
            if (b.Mantissa != 0)
            {
                result.Mantissa = a.Mantissa / b.Mantissa;
                result.E10 = a.E10 - b.E10;
            }
            return result;
        }

        public static bool operator >(BigDigit a, BigDigit b) { return a.CheckCount(b); }

        public static bool operator <(BigDigit a, BigDigit b) { return b.CheckCount(a); }
    }
}
