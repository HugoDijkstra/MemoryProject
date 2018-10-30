using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryProjectFull
{

    /// <summary>
    /// Static Wrapper Class for a specific MemoryConfig.
    /// </summary>
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

        /// <summary>
        /// Determines whether the MainConfig has a group with the specified name.
        /// </summary>
        /// <param name="name">The name of the group to look for.</param>
        /// <returns>True if the groups was found, false otherwise.</returns>
        public static bool HasGroup(string name)
        {
            return config_internal.HasGroup(name);
        }

        /// <summary>
        /// Adds a new group with the specified name to the MainConfig.
        /// </summary>
        /// <param name="name">The name of the group to add</param>
        /// <returns>The group that was added.</returns>
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

        /// <summary>
        /// Gets a group with the specified name. A new group is added if it does not exist.
        /// </summary>
        /// <param name="name">The name of the group to return.</param>
        /// <returns>The group with the specified name.</returns>
        public static DataGroup GetGroup(string name)
        {
            return config_internal.GetGroup(name);
        }

        /// <summary>
        /// Removes a group with the specified name.
        /// </summary>
        /// <param name="name">The name of the group to remove.</param>
        /// <returns>True if the group was removed, false otherwise</returns>
        public static bool RemoveGroup(string name)
        {
            return config_internal.RemoveGroup(name);
        }

        public static IEnumerable<DataGroup> Groups
        {
            get { return config_internal.Groups; }
        }

        /// <summary>
        /// Saves the MainConfig to a file.
        /// </summary>
        /// <returns></returns>
        public static bool Save()
        {
            return config_internal.SaveToFile(FILE_PATH);
        }

        private static MemoryConfig config_internal;

    }
}
