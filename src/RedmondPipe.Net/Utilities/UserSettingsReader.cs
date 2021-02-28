using Newtonsoft.Json;
using RedmondPipe.Models;
using System.IO;
using System.Windows.Forms;

namespace RedmondPipe.Utilities
{
    internal static class UserSettingsReader
    {
        private static string GetLocalAppSetings()
        {
#if DEBUG
            return Path.Combine(Application.StartupPath, "appsettings.devel.json");
#else
            return Path.Combine(Application.StartupPath, "appsettings.json");
#endif
        }

        private static string GetBestFilename(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                return GetLocalAppSetings();
            }

            if (!File.Exists(filename))
            {
                return GetLocalAppSetings();
            }

            return filename;
        }

        internal static AppSettingsModel ReadFromFile(string filename = "")
        {
            filename = GetBestFilename(filename);

            using (var sr = new StreamReader(filename))
            {
                var json = sr.ReadToEnd();
                return JsonConvert.DeserializeObject<Models.AppSettingsModel>(json);
            }
        }
    }
}
