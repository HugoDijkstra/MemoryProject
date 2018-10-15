using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMemoryGame
{
    class TurnManager
    {
        private static TurnManager instance;
        private List<Player> players;

        int numberPlayers = 4;

        public static TurnManager Instance()
        {
            if (instance == null) instance = new TurnManager();
            return instance;
        }

        private TurnManager()
        {
            players = new List<Player>();
            for(int i= 0; i < numberPlayers; i++)
            {
                players.Add(new Player("name", i));
            }
        }

        private void Turn()
        {

        }

        public List<Player> getPlayers()
        {
            return players;
        }
    }
}
