using System.Globalization;
using UniverseRift.Controllers.Common;

namespace UniverseRift.Heplers.Utils
{
    public static class DateTimeUtils
    {
        public static DateTime TryParseOrNow(string dateJson)
        {
            var result = DateTime.UtcNow;
            if (!string.IsNullOrEmpty(dateJson))
            {
                result = DateTime.ParseExact(
                    dateJson,
                    Constants.Common.DateTimeFormat,
                    CultureInfo.InvariantCulture
                );
            }

            return result;
        }
    }
}
