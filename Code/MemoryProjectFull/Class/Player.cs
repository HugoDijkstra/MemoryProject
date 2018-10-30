using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMemoryGame{

    class Player{
        public int ID;
        public string name;
        public int nextID;
        public int points;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="_name">client name</param>
        /// <param name="_ID">client id</param>
        /// <param name="_nextID">client next id</param>
        public Player(string _name, int _ID, int _nextID){
            name = _name;
            ID = _ID;
            nextID = _nextID;
            points = 0;
        }
    }
}
