using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMemoryGame
{
    class Player
    {
        public int ID;
        public string name;
        //int score;

        public int nextID;

        public Player(string _name, int _ID, int _nextID)
        {
            name = _name;
            ID = _ID;
            nextID = _nextID;
        }
    }
}
