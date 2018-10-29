using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryProjectFull
{
    public static class MainConfig
    {

        private static readonly string APP_FOLDER = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MemoryGame");
        private const string CONFIG_NAME = "main.mcfg";

        private static readonly string FILE_PATH = Path.Combine(APP_FOLDER, CONFIG_NAME);

        static MainConfig()
        {
            if (!Directory.Exists(APP_FOLDER)) Directory.CreateDirectory(APP_FOLDER);

            config_internal = MemoryConfig.LoadFromFile(FILE_PATH) ?? new MemoryConfig();
        }

        public static bool HasGroup(string name)
        {
            return config_internal.HasGroup(name);
        }

        public static DataGroup AddGroup(string name)
        {
            try
            {
                config_internal.AddGroup(name);
                return config_internal.GetGroup(name);
            }
            catch
            {
                throw;
            }
        }

        public static DataGroup GetGroup(string name)
        {
            return config_internal.GetGroup(name);
        }

        public static bool RemoveGroup(string name)
        {
            return config_internal.RemoveGroup(name);
        }

        public static IEnumerable<DataGroup> Groups
        {
            get { return config_internal.Groups; }
        }

        public static bool Save()
        {
            return config_internal.SaveToFile(FILE_PATH);
        }

        private static MemoryConfig config_internal;

    }
}
