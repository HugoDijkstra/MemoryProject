using MemoryProjectFull;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMemoryGame
{
    class TurnManager
    {
        NetworkCommand nxtTrnCmd;
        private List<Player> players;
        private GamePanel gamepanel;

        public TurnManager(string[] _names, int[] _id, GamePanel _gamepanel)
        {
            gamepanel = _gamepanel;
            gamepanel.onClickDone += new EventHandler<GamePanel.OnClickDoneArgs>(EndTurn);

            players = new List<Player>();
            for(int i= 0; i < _names.Length; i++)
            {
                int nextID = i + 1 >= _names.Length ? _id[0] : _id[i + 1];
                players.Add(new Player(_names[i], _id[i], nextID));
            }
            nxtTrnCmd = new NetworkCommand("G:NTURN", Turn, false, true);
        }

        private void Turn(string[] _data)
        {
            if (int.Parse(_data[0]) == NetworkHandler.getInstance().networkID){
                gamepanel.Activate();
                
            } else {
                gamepanel.Deactivate();
            } 
        }

        private void EndTurn(Object _sender, GamePanel.OnClickDoneArgs _onClickDoneArgs)
        {
            nxtTrnCmd.send(players.Find(x => x.ID == NetworkHandler.getInstance().networkID).nextID.ToString());
        }

        public List<Player> getPlayers()
        {
            return players;
        }
    }
}
