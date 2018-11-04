using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;
using System.Net;
using System.Net.Sockets;

/*
 * Command struckture
 * 
 * G = global
 * P = private
 * 
 * AA/AB/AC/BB/enz = command
 * 
 * G/P:AA <- command key
 * 
 * bleh,blah,bloeh <- command data
 * 
 */

public enum NetworkType {
    None,
    Host,
    Client
}

// G:NCON,0,bob

public class NetworkCommand{

    public string key;
    public Action<string[]> callback;

    public bool idCheck;

    /// <summary>
    /// constructor of NetworkCommand
    /// </summary>
    /// <param name="_key">command key</param>
    /// <param name="_callback">callback of command</param>
    /// <param name="_idCheck">do id check</param>
    /// <param name="_autoActiate">auto activate command</param>
    public NetworkCommand(string _key, Action<string[]> _callback, bool _idCheck = false, bool _autoActiate = false){
        key = _key;
        callback += _callback;
        idCheck = _idCheck;

        if (_autoActiate)
            activate();
    }
    
    /// <summary>
    /// send a command to all clients
    /// </summary>
    /// <param name="_data">data of command</param>
    public void send(string[] _data) {
        NetworkManager.getInstance().send(key, _data);
    }

    /// <summary>
    /// send a command to all clients
    /// </summary>
    /// <param name="_data">data of command</param>
    public void send(string _data) {
        NetworkManager.getInstance().send(key, new string[1] { _data });
    }

    /// <summary>
    /// activate command
    /// </summary>
    public void activate() {
        NetworkHandler.getInstance().addCommand(this);
    }

    /// <summary>
    /// deactivate command
    /// </summary>
    public void disable() {
        NetworkHandler.getInstance().removeCommmand(this);
    }
}

/// <summary>
/// The network handler class
/// </summary>
public class NetworkHandler {
    private static NetworkHandler _instance;

    // connections with new clients
    public Action OnLostConnectionToHost;

    public Action<int> OnResyncRequest;

    public Action<int> OnInitialized;
    public Action<int> OnClientConnection;
    public Action<int> OnClientDisconection;

    private List<NetworkCommand> _serverCommands;
    private List<NetworkCommand> _cleintCommands;

    // vars for this client
    public int networkID;

    // server commands
    NetworkCommand _serverQuit;
    NetworkCommand _serverInit;
    NetworkCommand _serverSyncConnection;
    NetworkCommand _serverSyncRequest;
    NetworkCommand _serverDisconectHost;
    NetworkCommand _serverConnect;
    NetworkCommand _serverDisconnect;

    /// <summary>
    /// constructor
    /// </summary>
    public NetworkHandler() {
        _serverCommands = new List<NetworkCommand>();
        _cleintCommands = new List<NetworkCommand>();

        _serverQuit = new NetworkCommand("S:QUIT", (x) => {
            Environment.Exit(1); // temp
        });

        _serverInit = new NetworkCommand("S:INIT", (x) => {
            networkID = int.Parse(x[0]);
            OnInitialized(networkID);
            //_serverConnect.send(networkID.ToString());
        });

        _serverDisconectHost = new NetworkCommand("S:NOHOST", (x) => {
            OnLostConnectionToHost.Invoke();
        });

        _serverSyncConnection = new NetworkCommand("S:SYNC", (x) => { // call the request resync callback
            OnResyncRequest?.Invoke(int.Parse(x[0]));
        });

        _serverSyncRequest = new NetworkCommand("S:RSYNC", (x) => { // send resync request to all clients but [id] (only host can do this)
            //resyncClient(int.Parse(x[0]));

            if (isHost()) {
                ClientConnection[] clients = NetworkManager.getInstance().getClientConnections();
                int length = clients.Length;
                for (int i = 0; i < length; i++) {
                    if(clients[i].networkID != int.Parse(x[0]))
                        NetworkManager.getInstance().sendTargetCommand(clients[i].networkID, _serverSyncConnection, x);
                }
                if (-1 != int.Parse(x[0]))
                    OnResyncRequest?.Invoke(int.Parse(x[0]));
            }
        });

        _serverConnect = new NetworkCommand("S:CONNECT", (x) => {
            OnClientDisconection?.Invoke(int.Parse(x[0]));
        });

        _serverDisconnect = new NetworkCommand("S:DISCONNECT", (x) => {
            OnClientDisconection?.Invoke(int.Parse(x[0]));
        });

        _serverCommands.Add(_serverQuit);
        _serverCommands.Add(_serverInit);
        _serverCommands.Add(_serverDisconectHost);
        _serverCommands.Add(_serverSyncConnection);
        _serverCommands.Add(_serverSyncRequest);
        _serverCommands.Add(_serverConnect);
        _serverCommands.Add(_serverDisconnect);
    }

    /// <summary>
    /// deconstructor
    /// </summary>
    ~NetworkHandler() {
        this.terminate();
    }

    /// <summary>
    /// init function
    /// </summary>
    /// <param name="_id">network id</param>
    public void init(int _id) {
        _serverInit.callback?.Invoke(new string[1] { _id.ToString() });
    }

    /// <summary>
    /// get instance function
    /// </summary>
    /// <returns>instance of class</returns>
    public static NetworkHandler getInstance() {
        if (_instance == null) {
            _instance = new NetworkHandler();
        }
        return _instance;
    }
    
    /// <summary>
    /// add new command
    /// </summary>
    /// <param name="_command">network command</param>
    public void addCommand(NetworkCommand _command) {
        if (!_cleintCommands.Contains(_command)) // <-- can be removed???
            _cleintCommands.Add(_command);
    }

    /// <summary>
    /// remove command
    /// </summary>
    /// <param name="_command">network command</param>
    public void removeCommmand(NetworkCommand _command) {
        if (_cleintCommands.Contains(_command)) // <-- can be removed???
            _cleintCommands.Remove(_command);
    }

    /// <summary>
    /// get if this is host
    /// </summary>
    /// <returns>is host</returns>
    public bool isHost() {
        return NetworkManager.getInstance().networkType == NetworkType.Host;
    }

    /// <summary>
    /// request a resync
    /// </summary>
    public void requestResync() {
        _serverSyncRequest.send(new string[1] { networkID.ToString() });
    }

    /// <summary>
    /// terminate
    /// </summary>
    public void terminate() {
        _serverCommands.Clear();
        _cleintCommands.Clear();
    }

    /// <summary>
    /// on recieve command
    /// </summary>
    /// <param name="_command">command id</param>
    /// <param name="_data">command data</param>
    public void onRecieveCommand(string _command, string[] _data){
        for (int i = 0; i < _serverCommands.Count(); i++){
            if (_serverCommands[i].key == _command) {
                _serverCommands[i].callback?.Invoke(_data);
                return;
            }
        }

        for (int i = 0; i < _cleintCommands.Count(); i++){
            if (_cleintCommands[i].key == _command) {

                if (_cleintCommands[i].idCheck){
                    if (networkID != int.Parse(_data[0])){
                        _cleintCommands[i].callback?.Invoke(_data);
                    }
                    else { 
                        return;
                    }                    
                }
                else if (!_cleintCommands[i].idCheck) { 
                    _cleintCommands[i].callback?.Invoke(_data);
                }
            }
        }
    }

}

/// <summary>
/// NetworkManager class
/// </summary>
public class NetworkManager {
    private static NetworkManager _instance;
    private static int _uniqueID;

    // callback to recieve a message from the host
    public Action<string, string[]> OnRecieveCommand;

    private TcpListener _host;

    private Thread _checkConnectionThread;
    private Thread _createConnectionThread;

    private List<ClientConnection> _clientConnections;
    private ClientConnection _clientConnection;

    public NetworkType networkType;

    /// <summary>
    /// constructor
    /// </summary>
    private NetworkManager() {
        _clientConnections = new List<ClientConnection>();
        networkType = NetworkType.None;
        _uniqueID = 0;
    }

    /// <summary>
    /// deconstructor
    /// </summary>
    ~NetworkManager() {
        this.terminate();
    }

    /// <summary>
    /// get instance of class
    /// </summary>
    /// <returns>the instance</returns>
    public static NetworkManager getInstance() {
        if (_instance == null) {
            _instance = new NetworkManager();
        }
        return _instance;
    }

    /// <summary>
    /// create a connection
    /// </summary>
    /// <param name="_type">network type</param>
    /// <param name="_ip">ip</param>
    /// <param name="_port">port</param>
    /// <param name="_OnInitialized">callback</param>
    public void create(NetworkType _type, string _ip, int _port, Action<int> _OnInitialized) {
        create(_type, _ip, _port, _OnInitialized, 16384);
    }

    /// <summary>
    /// create a connection
    /// </summary>
    /// <param name="_type">network type</param>
    /// <param name="_ip">ip</param>
    /// <param name="_port">port</param>
    /// <param name="_OnInitialized">callback</param>
    /// <param name="_bufferSize">buffer size</param>
    public void create(NetworkType _type, string _ip, int _port, Action<int> _OnInitialized, int _bufferSize) {
        networkType = _type;

        OnRecieveCommand += NetworkHandler.getInstance().onRecieveCommand;
        NetworkHandler.getInstance().OnInitialized += _OnInitialized;
        NetworkHandler.getInstance().OnClientDisconection += OnClientDisconection;

        if (_type == NetworkType.None) {
            return;
        }
        else if (_type == NetworkType.Host) {
            // create host connection
            _host = new TcpListener(new IPEndPoint(IPAddress.Any, _port));
            _host.Start();

            // activate connection listen thread
            _checkConnectionThread = new Thread(new ThreadStart(onClientConnect));
            _checkConnectionThread.Start();

            // because this is host he init's on creation
            NetworkHandler.getInstance().init(-1);
        }
        else if (_type == NetworkType.Client){
            _createConnectionThread = new Thread(new ThreadStart(() => {
                TcpClient tcpClient = new TcpClient();

                while (true) {
                    try{
                        tcpClient.Connect(_ip, _port);
                        break;
                    }
                    catch (Exception){
                    }
                }
                
                _clientConnection = new ClientConnection(tcpClient, _bufferSize);
                _clientConnection.OnConnectionTerminate += terminateClient;
                _clientConnection.OnReceiveData += recieveData;

                _createConnectionThread.Abort();
            }));
            
            NetworkHandler.getInstance().networkID = -2;
            _createConnectionThread.Start();
        }
    }

    /// <summary>
    /// get all client connections
    /// </summary>
    /// <returns></returns>
    public ClientConnection[] getClientConnections() {
        return _clientConnections.ToArray();
    }

    /// <summary>
    /// send targeted command
    /// </summary>
    /// <param name="_id">cleint id</param>
    /// <param name="_command">network command</param>
    /// <param name="_data">command data</param>
    public void sendTargetCommand(int _id, NetworkCommand _command, string[] _data) {
        int length = _clientConnections.Count;
        for (int i = 0; i < length; i++){
            if (_clientConnections[i].networkID == _id) {
                string message = formatCommand(_command.key, _data);
                _clientConnections[i].send(message);
                return;
            }
        }
    }

    /// <summary>
    /// get if client is host
    /// </summary>
    /// <returns>is host</returns>
    private bool isHost() {
        return networkType == NetworkType.Host;
    }

    /// <summary>
    /// format the command
    /// </summary>
    /// <param name="_id">command id</param>
    /// <param name="_data">command data</param>
    /// <returns></returns>
    private string formatCommand(string _id, string[] _data) {
        string message = _id + ",";
        for (int i = 0; i < _data.Length; i++){
            message += _data[i];
            if (i < _data.Length-1) {
                message += ",";
            }
        }
        return message;
    }

    /// <summary>
    /// send command to all clients
    /// </summary>
    /// <param name="_id">command id</param>
    /// <param name="_data">command data</param>
    public void send(string _id, string[] _data) {
        // format data
        string message = formatCommand(_id, _data);
        
        string debug = "";
        for (int i = 0; i < _data.Length; i++){
            debug += _data[i] + ", ";
        }
        Console.WriteLine(string.Format("[{1}] SENDING -> {0}", message, DateTime.UtcNow.Ticks.ToString()));

        if (networkType == NetworkType.Host) {
            // send data to all clients
            for (int i = 0; i < _clientConnections.Count; i++){
                _clientConnections[i].send(message);
            }

            // send data to self
            recieveData(message);
        }else if (networkType == NetworkType.Client){
            _clientConnection.send(message);
        }
    }
    
    /// <summary>
    /// terminate
    /// </summary>
    public void terminate() {
        NetworkHandler.getInstance().OnClientDisconection += OnClientDisconection;
        
        if (_host != null) {
            _host.Stop();
        }

        if (_checkConnectionThread != null) {
            _checkConnectionThread.Abort();
            _checkConnectionThread = null;
        }
        
        if (_createConnectionThread != null){
            _createConnectionThread.Abort();
            _createConnectionThread = null;
        }

        for (int i = 0; i < _clientConnections.Count; i++){
            ClientConnection c = _clientConnections[i];
            _clientConnections.RemoveAt(i);
            c.terminate();
        }

        if (_clientConnection != null) {
            _clientConnection.terminate();
            _clientConnection = null;
        }
    }

    /// <summary>
    /// recieve data callback
    /// </summary>
    /// <param name="_message">command data compact</param>
    private void recieveData(string _message) {
        string[] array = _message.Split(',');
        string command = array[0];
        string[] data = array.Skip(1).ToArray();
        
        string message = formatCommand(command, data);
        Console.WriteLine(string.Format("[{1}] RECIEVING -> {0}", message, DateTime.UtcNow.Ticks.ToString()));

        OnRecieveCommand?.Invoke(command, data);
    }

    /// <summary>
    /// on client connection callback
    /// </summary>
    private void onClientConnect() {
        while (true) { 
            TcpClient client = _host.AcceptTcpClient();
            Console.WriteLine("Connected to new client");
            ClientConnection cConnection = new ClientConnection(client, 1024);
             
            cConnection.OnConnectionTerminate += terminateClient;
            cConnection.OnReceiveData += (x) => {

                string[] array = x.Split(',');
                string command = array[0];
                string[] data = array.Skip(1).ToArray();
                send(command, data);

            };

            // send client id to client ( this init's the client )
            int networkID = _uniqueID;
            _uniqueID++;

            cConnection.send("S:INIT," + networkID);
            cConnection.networkID = networkID;
            _clientConnections.Add(cConnection);
        }
    }
    
    /// <summary>
    /// disconect from client
    /// </summary>
    /// <param name="_id">client id</param>
    private void OnClientDisconection(int _id) {
        ClientConnection client = _clientConnections.Find(x => _id == x.networkID);
        if (client != null)
            terminateClient(client);
    }

    /// <summary>
    /// termiante client
    /// </summary>
    /// <param name="c">client</param>
    private void terminateClient(ClientConnection c) {
        if (isHost()) {
            int id = c.networkID;
            _clientConnections.Remove(c);
            c.terminate();
            send("S:DISCONNECT", new string[1] { id.ToString() });
        }else {
            recieveData("S:NOHOST");
        }
    }
}

public class ClientConnection{

    public int networkID;

    public Action<string> OnReceiveData;
    public Action<ClientConnection> OnConnectionTerminate;

    private TcpClient _tcpClient;
    private NetworkStream _stream;
    private byte[] _buffer;

    private Thread _clientListener;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="_tcpC">tcp client</param>
    /// <param name="_bufferSize">buffer size</param>
    public ClientConnection(TcpClient _tcpC, int _bufferSize) {
        _tcpClient = _tcpC;
        _stream = _tcpClient.GetStream();

        _buffer = new byte[_bufferSize];

        _clientListener = new Thread(new ThreadStart(listen));
        _clientListener.Start();
    }

    /// <summary>
    /// send data
    /// </summary>
    /// <param name="_message">data</param>
    public void send(string _message) {
        if(_tcpClient != null) { 
            byte[] byteMessage = Encoding.ASCII.GetBytes(_message);
            _stream.Write(byteMessage, 0, byteMessage.Length);
        }
    }

    /// <summary>
    /// listen to the server and prosses the messages
    /// </summary>
    private void listen() {
        string message = "";
        int length = 0;

        while (true){
            try{
                length = _stream.Read(_buffer, 0, _buffer.Length);
            }
            catch (Exception){
                break;
            }

            message = Encoding.ASCII.GetString(_buffer, 0, length);
            OnReceiveData?.Invoke(message);
        }

        OnConnectionTerminate?.Invoke(this);
    }

    /// <summary>
    /// stop the connection and abort the thread
    /// </summary>
    public void terminate() {
        if (_clientListener != null){
            _clientListener.Abort();
            _clientListener = null;
        }

        if (_tcpClient != null) {
            _tcpClient.Close();
        }

        Console.WriteLine("Stopped connection with client");
    }

}