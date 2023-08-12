namespace UniverseRift.Heplers.MathOperations
{
    public static partial class CustomMath
    {
        public static int RoundToNearestInt(float f)
        {
            var previousNumber = (int)f;
            var nextNumber = previousNumber + 1;

            var downDelta = f - previousNumber;
            var upDelta = nextNumber - f;
            if (downDelta < upDelta)
            {
                return previousNumber;
            }
            else
            {
                return nextNumber;
            }
        }
    }
}
