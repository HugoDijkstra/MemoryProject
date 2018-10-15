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
        int score;

       // TextBox playerInfo;

        public Player(string _name, int _ID)
        {
            name = _name;
            ID = _ID;
        }

       /* public TextBox ShowPlayerInformation()
        {
            playerInfo = new TextBox();
            playerInfo.Text = "Name: " + name;
            return playerInfo;
        }*/
    }
}
