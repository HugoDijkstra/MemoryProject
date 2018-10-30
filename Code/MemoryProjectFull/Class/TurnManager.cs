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

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="_players">players list</param>
        /// <param name="_playerinfo">player info list</param>
        /// <param name="_gamepanel">game panel</param>
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

        /// <summary>
        /// on next turn network callback
        /// </summary>
        /// <param name="_data">command data</param>
        private void Turn(string[] _data){

            // fix turn indicator
            int id = int.Parse(_data[0]);
            for (int i = 0; i < players.Count; i++){
                if (players[i].ID == id){
                    playerPanels[i].SetTurn(true);
                    int old = i - 1 < 0 ? players.Count - 1 : i - 1;
                    playerPanels[old].SetTurn(false);
                }
            }

            // on correct card
            if (_data[1] == "1")
            {
                //AudioManager.GetAudio("card_done").Play(false);
                gamepanel.RemoveCard(int.Parse(_data[2]));

                for (int i = 0; i < players.Count; i++)
                {
                    if (players[i].ID == id)
                    {
                        // add points to player
                        players[i].points++;
                        playerPanels[i].SetCards(players[i].points);

                        // on grid empty
                        if (gamepanel.IsGridEmpty())
                        {
                            OnGameEnded?.Invoke(players[i].name, players[i].points);
                            return;
                        }
                        break;
                    }
                }
            }
            else {
                //AudioManager.GetAudio("card_fail").Play(false);
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

        /// <summary>
        /// on end turn callback
        /// </summary>
        /// <param name="_sender"></param>
        /// <param name="_onClickDoneArgs"></param>
        private void EndTurn(Object _sender, GamePanel.OnClickDoneArgs _onClickDoneArgs){
            Console.WriteLine("END TUNR!!!!");

            string userID = players.Find(x => x.ID == NetworkHandler.getInstance().networkID).nextID.ToString();
            string correct = _onClickDoneArgs.Correct ? "1" : "0";
            string cardID = _onClickDoneArgs.firstCard.ID.ToString();

            nxtTrnCmd.send(new string[3] { userID, correct, cardID });
        }

        /// <summary>
        /// get list of players
        /// </summary>
        /// <returns>players list</returns>
        public List<Player> getPlayers(){
            return players;
        }
    }
}
