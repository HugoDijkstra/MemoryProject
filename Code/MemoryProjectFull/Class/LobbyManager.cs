using NewMemoryGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class LobbyManager {

    public Action<Player[]> OnStart;

    private List<Client> global;
    private Client local;

    private NetworkCommand _onResync;
    private NetworkCommand _onConnection;

    public LobbyManager(string _clientName) {
        global = new List<Client>();
        
        string name = _clientName;
        local = new Client(name, NetworkHandler.getInstance().networkID);

        _onResync = new NetworkCommand("G:RSYNC", (x) => {

            if (NetworkHandler.getInstance().isHost()){ // <-- make format function for this or make sending data beter (sending arrays and formating them in Net)
                int count = global.Count * 2 + 4; // id, count, local user, users[]
                string[] data = new string[count];
                data[0] = NetworkHandler.getInstance().networkID.ToString();
                data[1] = (global.Count + 1).ToString();

                data[2] = local.id.ToString();
                data[3] = local.name;

                int globalCount = 0;
                for (int i = 4; i < count; i += 2){
                    data[i] = global[globalCount].id.ToString();
                    data[i + 1] = global[globalCount].name;
                    globalCount++;
                }

                Console.WriteLine("Resync send!!!!");
                _onResync.send(data);
            }
            else {
                int count = int.Parse(x[1]) * 2 + 2;

                for (int i = 2; i < count; i += 2){
                    OnConnect(int.Parse(x[i]), x[i + 1]);
                }

                _onConnection.send(new string[2] { local.id.ToString(), local.name });
                _onResync.disable();
            }
        }, true, true);

        _onConnection = new NetworkCommand("G:NCON", (x) => {
            int id = int.Parse(x[0]);
            string cName = x[1];

            for (int i = 0; i < global.Count; i++){ // <-- might remove this (test it fam)
                if (global[i].id == id)
                    return;
            }

            OnConnect(id, cName);
        }, true, true);

        _onResync.send(NetworkHandler.getInstance().networkID.ToString());
    }

    public void startGame() { // clean this code or find a way to do it better
        List<Player> players = new List<Player>();
        global.Add(local);

        int count = global.Count;

        Client client = null;
        int lowestID = int.MaxValue;

        for (int i = 0; i < count; i++){
            for (int j = 0; j < global.Count; j++) {
                if (global[i].id < lowestID) {
                    client = global[i];
                    lowestID = client.id;
                }
            }

            lowestID = int.MaxValue;
            i--;
            global.Remove(client);
            count = global.Count;
            players.Add(new Player(client.name, client.id, 0));
        }

        for (int i = 0; i < players.Count; i++){
            int id = i + 1 >= players.Count ? players[0].ID : players[i + 1].ID;
            players[i].nextID = id;
        }

        OnStart?.Invoke(players.ToArray());
    }

    // add new client to global
    private void OnConnect(int _id, string _name) {
        global.Add(new Client(_name, _id));
    }
}

class Client {
    public string name;
    public int id;

    public Client(string _name, int _id) {
        name = _name;
        id = _id;
    }
}
