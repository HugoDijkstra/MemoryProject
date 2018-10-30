using MemoryProjectFull;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NewMemoryGame{
    class TurnManager{

        private NetworkCommand nxtTrnCmd;
        private List<Player> players;
        private GamePanel gamepanel;

        private List<PlayerInfo> playerPanels;

        public TurnManager(Player[] _players, List<PlayerInfo> _playerinfo, GamePanel _gamepanel){
            gamepanel = _gamepanel;
            gamepanel.Deactivate();

            playerPanels = _playerinfo;

            players = _players.ToList(); // <-- first i do to array and then to list? might need fix?
            nxtTrnCmd = new NetworkCommand("G:NTURN", Turn, false, true);

            if (NetworkHandler.getInstance().isHost()) { // <-- what if we have dedicated server?
                nxtTrnCmd.send(new string[3] { _players[0].ID.ToString(), "0", "0"});
            }
        }

        private void Turn(string[] _data){

            if (_data[1] == "1") {
                gamepanel.RemoveCard(int.Parse(_data[2]));
            }

            Console.WriteLine("NEXT TUNR!!!!");
            if (int.Parse(_data[0]) == NetworkHandler.getInstance().networkID){
                gamepanel.Activate();
                gamepanel.onClickDone += new EventHandler<GamePanel.OnClickDoneArgs>(EndTurn);
                Console.WriteLine("YOU CAN MOVE!");
            } else {
                gamepanel.Deactivate();
                gamepanel.onClickDone -= new EventHandler<GamePanel.OnClickDoneArgs>(EndTurn);
                Console.WriteLine("YOU CAN'T MOVE!");
            } 
        }

        private void EndTurn(Object _sender, GamePanel.OnClickDoneArgs _onClickDoneArgs){
            Console.WriteLine("END TUNR!!!!");

            string userID = players.Find(x => x.ID == NetworkHandler.getInstance().networkID).nextID.ToString();
            string correct = _onClickDoneArgs.Correct ? "1" : "0";
            string cardID = _onClickDoneArgs.firstCard.ID.ToString();

            nxtTrnCmd.send(new string[3] { userID, correct, cardID });
        }

        public List<Player> getPlayers()
        {
            return players;
        }
    }
}
