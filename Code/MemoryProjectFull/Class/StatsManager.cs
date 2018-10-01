using System;
using System.Collections.Generic;
using System.IO;
using Jil;

using Database = System.Collections.Generic.Dictionary<string, MemoryProjectFull.Stats>;

namespace MemoryProjectFull
{

    public class StatsManager
    {

        public StatsManager()
        {
            this.database = new Database();
        }

        public static StatsManager LoadFromJSON(string filePath)
        {
            StatsManager statsManager = new StatsManager();

            if (File.Exists(filePath))
            {
                using (var fstream = File.OpenRead(filePath))
                using (var input = new StreamReader(fstream))
                {
                    statsManager.database = JSON.Deserialize<Database>(input);
                }
            }
            else
            {
                using (var fstream = File.Create(filePath))
                using (var output = new StreamWriter(fstream))
                {
                    output.Write(JSON.Serialize<Database>(statsManager.database));
                }
            }

            return statsManager;
        }

        public void WriteToJSON(string filePath)
        {
            File.WriteAllText(filePath, string.Empty); //Clear the file before writing.

            using (var fstream = File.OpenWrite(filePath))
            using (var output = new StreamWriter(fstream))
            {
                output.Write(JSON.Serialize<Database>(database));
            }
        }

        public bool StoreStats(Stats stats)
        {
            if (PlayerExists(stats.Name)) return false;

            database.Add(stats.Name, stats);

            return true;
        }

        public bool PlayerExists(string name)
        {
            return database.ContainsKey(name);
        }

        public Stats RetrieveStats(string name)
        {
            if (!PlayerExists(name)) return null;

            return database[name];
        }

        public bool RemoveStats(string name)
        {
            if (!PlayerExists(name)) return false;

            database.Remove(name);
            return true;
        }

        private Database database;

    }
}
