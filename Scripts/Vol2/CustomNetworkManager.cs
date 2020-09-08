using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField] private int minPlayers = 2;
    [Scene] [SerializeField] private string menuScene = string.Empty; 

    [Header("Room")]
    //Referoidaan "Huone" Pelaajaa
    [SerializeField] private NetWorkRoomFuturace roomPlayerPrefab = null;

    [Header("Game")]
    //Gameversion of prefab Referointi
    [SerializeField] private NetworkGameFuturace gamePlayerPrefab = null;
    [SerializeField] private GameObject playerSpawnSystem = null;
    [SerializeField] private GameObject roundSystem = null;
    [SerializeField] private GameObject DataCollecter = null;


    
    public static event Action onClientConnected;
    public static event Action onClientDisconnected;

    public static event Action<NetworkConnection> OnServerReadied;
    // Tietääksemme koska serveri pysähtyy
    public static event Action OnServerStopped;

    //Luodaan lista huoneessa ja pelissä olevista pelaajista
    public List<NetWorkRoomFuturace> RoomPlayers {get;} = new List<NetWorkRoomFuturace>();
    public List<NetworkGameFuturace> GamePlayers {get;} = new List<NetworkGameFuturace>();

    //Ladataan jokainen spawnautuva prefab Serverille ja clientille ilman että sitä tarvitsee lisäillä erikseen networkmanagerissa
    public override void OnStartServer() => spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();

    public override void OnStartClient(){
        var SpawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");

        foreach (var prefab in SpawnablePrefabs){
            ClientScene.RegisterPrefab(prefab);
        }
    }
    public override void OnClientConnect(NetworkConnection conn){
        base.OnClientConnect(conn);

        onClientConnected?.Invoke();
    }

    public override void OnClientDisconnect(NetworkConnection conn){
        base.OnClientDisconnect(conn);

        onClientDisconnected?.Invoke();
    }

    public override void OnServerConnect(NetworkConnection conn){
        //Jos liikaa pelaajia pelissä
        if (numPlayers >= maxConnections){
            conn.Disconnect();
            return;
        }
        //jos aktiivinen scene ei ole menuscene eli ollaan pelissä, niin mukaan ei pääse
        if (SceneManager.GetActiveScene().path !=menuScene){
            conn.Disconnect();
            return;
        }
    }
    //Kun serveri lisää pelaajan
    public override void OnServerAddPlayer(NetworkConnection conn){
        if (SceneManager.GetActiveScene().path == menuScene){
            //Kun tämä henkilö liittyy jos hän on listassa sijalla 0 hän on johtaja
            bool isLeader = RoomPlayers.Count == 0;
            //Luodaan instanssi "Huone"Pelaajasta
            NetWorkRoomFuturace roomPlayerInstance = Instantiate(roomPlayerPrefab);
            //Tämä on sitä varten että clientille voidan kertoa onko hän johtaja
            roomPlayerInstance.IsLeader = isLeader;
            //Yhdistetään connectioni pelaajalle.
            NetworkServer.AddPlayerForConnection(conn,roomPlayerInstance.gameObject);
        }
    }
    public override void OnServerDisconnect(NetworkConnection conn){
        if (conn.identity != null){
            //Haetaan poistuneen pelaajan RoomFuturace scripti
            var player = conn.identity.GetComponent<NetWorkRoomFuturace>();
            //Ja poistetaan pelaaja listasta (vain serveri)
            RoomPlayers.Remove(player);
            NotifyPlayersOfReadyState();
        }
        //Basessa tuhotaan oikeasti yhteys
        base.OnServerDisconnect(conn);
    }
    public override void OnStopServer(){
        //Serveri sammutetaan Huone tyhjennetään
        OnServerStopped?.Invoke();

        RoomPlayers.Clear();
        GamePlayers.Clear();
    }
    public void NotifyPlayersOfReadyState(){
        foreach (var player in RoomPlayers){
            //Jokaisella pelaajalla ajetaan metodi onkovalmis aloittamaan jolle annetaan boolean onValmisAloittamaan
            player.HandleReadyToStart(IsReadyToStart());
        }
    }
    private bool IsReadyToStart(){
        //jos pelaajian on liian vähän eli alle 2 niin false
        if (numPlayers < minPlayers){return false;}
        //jos yksikään pelaajista ei ole valmis -> false
        foreach (var player in RoomPlayers){
            if (!player.IsReady){return false;}
        }
        //Pelaajia tarpeeksi, kaikki valmiina -> true
        return true;
    }

    public void StartGame(){
        if (SceneManager.GetActiveScene().path == menuScene){
            //Jos emme ole valmiita aloittamaan
            if (!IsReadyToStart()){return;}
            //Jos UI Lobbyyn laitetaan Vaihtoehdot niin tähän päivittyisi se
            ServerChangeScene("DesertCastle");
        }
    }
    public override void ServerChangeScene(string newSceneName){
        //Menusta peliin
        if (SceneManager.GetActiveScene().path == menuScene && newSceneName.StartsWith("DesertCastle") || newSceneName.StartsWith("Victory")){
            for (int i = RoomPlayers.Count -1; i>=0;i--){
                var conn = RoomPlayers[i].connectionToClient;
                //Spawnataan peliversio pelaajasta
                var gameplayerInstance = Instantiate(gamePlayerPrefab);
                //Siirettän nimi peliversioon pelaajasta
                gameplayerInstance.SetDisplayName(RoomPlayers[i].DisplayName);
                // Tässä tuhotaan Room player
                NetworkServer.Destroy(conn.identity.gameObject);
                //Tässä paikataan pelaajan yhteys, eli uusi gameplayerInstance
                NetworkServer.ReplacePlayerForConnection(conn, gameplayerInstance.gameObject, true);

            }
        }
        base.ServerChangeScene(newSceneName);
    }

    public override void OnServerSceneChanged(string sceneName){
        if (sceneName.StartsWith("DesertCastle")){
            //Spawnataan playerSpawnSystem
            GameObject playerSpawnSystemInstance = Instantiate(playerSpawnSystem);
            NetworkServer.Spawn(playerSpawnSystemInstance);
            //SpawnataanRoundSystem
            GameObject roundSystemInstance = Instantiate(roundSystem);
            NetworkServer.Spawn(roundSystemInstance);
            GameObject DataCollecterInstance = Instantiate(DataCollecter);
            NetworkServer.Spawn(DataCollecterInstance);


        }
    }

    public override void OnServerReady(NetworkConnection conn){
        base.OnServerReady(conn);

        OnServerReadied?.Invoke(conn);
    }
}
