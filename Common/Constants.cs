using Newtonsoft.Json;

namespace UniverseRift.Controllers.Common
{
    public class Constants
    {
        public class Common
        {
            public static string SUCCESS_RUSULT = "Success";

            public static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            public static string DictionariesPath
            {
                get
                {
                    return "Jsons";
                }
            }

            public static string DateTimeFormat = "dd.MM.yyyy HH:mm:ss";

            public static int MAX_RANDOM = 1000000;
        }

        public class Game
        {
            public const int MINE_MISSION_REFRESH_HOURS = 8;
        }
    }
}