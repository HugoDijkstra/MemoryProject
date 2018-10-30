using MemoryProjectFull;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NewMemoryGame{
    class TurnManager{

        public Action<string, int> OnGameEnded;

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

            OnGameEnded += (x, y) => {
                Account.updateScore(x == Account.name); // boi!
            };
        }

        private void Turn(string[] _data){

            // on correct card
            if (_data[1] == "1") {
                gamepanel.RemoveCard(int.Parse(_data[2]));

                int id = int.Parse(_data[0]);
                for (int i = 0; i < players.Count; i++){
                    if (players[i].ID == id) {
                        // add points to player
                        players[i].points++;
                        playerPanels[i].SetCards(players[i].points);
                        
                        // on grid empty
                        if (gamepanel.IsGridEmpty()) {
                            OnGameEnded?.Invoke(players[i].name, players[i].points);
                            return;
                        }
                        break;
                    }
                }
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

        public List<Player> getPlayers(){
            return players;
        }
    }
}
