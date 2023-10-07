using UniverseRift.Controllers.Common;

namespace UniverseRift.Heplers.GameLogging
{
    public static class GameLogging
    {
        public static void WriteGameLog(string message)
        {
            var directoryPath = "Logs";
            var fileName = "UniverseRift_game_log.txt";
            var writer = TextUtils.GetFileWriterStream(directoryPath, fileName, true);

            writer.WriteLine($"{DateTime.UtcNow.ToString("MM.dd.yyyy HH:mm:ss.ff")}\n{message}\n");
            writer.Close();
            writer.Dispose();
        }
    }
}
