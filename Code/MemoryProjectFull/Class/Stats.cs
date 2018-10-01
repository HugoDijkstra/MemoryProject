using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryProject
{

    public class Stats
    {

        public Stats()
        {
            this.name = "";
            this.wins = 0;
            this.losses = 0;
        }

        public Stats(string name, uint wins, uint losses)
        {
            this.name = name;
            this.wins = wins;
            this.losses = losses;
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public uint Wins
        {
            get { return wins; }
            set { wins = value; }
        }

        public uint Losses
        {
            get { return losses; }
            set { losses = value; }
        }

        private string name;
        private uint wins;
        private uint losses;

    }

}
