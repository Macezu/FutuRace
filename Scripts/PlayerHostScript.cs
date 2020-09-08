using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;


public class PlayerHostScript : NetworkBehaviour
{
    //Staattinen alustus sille että olemmepelaaja Ui.lobbya varten
    public static PlayerHostScript localPlayer;
    public static bool isOnStartupLine = false;

    //Id kokoelma
    [SyncVar] public string matchID;
    [SyncVar] public int playerIndex;
    [SyncVar] public int spawnIndex;

    NetworkMatchChecker networkMatchChecker;
    [SyncVar] public Match currentMatch;

    GameObject playerLobbyUI;
    void Awake()
    {
        networkMatchChecker = GetComponent<NetworkMatchChecker>();
    }

    public override void OnStartClient()
    {
        if (isLocalPlayer)
        {
            localPlayer = this;
        }
        else
        {
            //spawnataan toisen pelaajan UI
            Debug.Log($"Spawning other player UI");
            playerLobbyUI = UILobby.instance.SpawnPlayerUIPrefab(this);
        }

    }

    public override void OnStopClient()
    {
        Debug.Log($"Client Stopped");
        ClientDisconnect();
    }
    public override void OnStopServer()
    {
        Debug.Log($"Client stopped on server");
        ServerDisconnect();
    }
    /*
    HOSTAAMINEN
    Kun hostataan saadaan MatchMakerilta ensimmäiseksi random id numero
    */
    public void hostGame(bool publicMatch)
    {
        string matchID = MatchMaker.getRandomID();
        CmdHostGame(matchID, publicMatch);
    }

    //Pelaaja saa peliID:n ja pyytää serveriä lisämään tämän id:n listaan aktiivistista peleistä
    [Command]
    void CmdHostGame(string _peliID, bool publicMatch)
    {
        //  outtaaminen suoraan muuttujalle =playerIndex;
        matchID = _peliID;
        if (MatchMaker.instance.HostGame(_peliID, gameObject, publicMatch, out playerIndex))
        {
            networkMatchChecker.matchId = _peliID.toGuid();
            TargetHostGame(true, _peliID, playerIndex);
        }
        else
        {
            Debug.Log("Game could not be hosted");
            TargetHostGame(false, _peliID, playerIndex);
        }
    }


    [TargetRpc]
    void TargetHostGame(bool success, string _peliID, int _playerIndex)
    {
        playerIndex = _playerIndex;
        spawnIndex = _playerIndex;
        matchID = _peliID;
        Debug.Log($"MatchId: {matchID} == {_peliID}");
        UILobby.instance.HostSuccess(success, _peliID);

    }

    /*
    JOINAAMINEN
    
    Otetaan Uilobbyn tekstikentästä tieto ja annetaan se parametriksi
    */
    public void joinGame(string _TMPinput)
    {
        CmdjoinGame(_TMPinput);
    }

    //saadaan käyttäjältä id ja joinataan peliin joka vasta kyseistä id:tä
    [Command]
    void CmdjoinGame(string _peliID)
    {
        matchID = _peliID;
        if (MatchMaker.instance.JoinGame(_peliID, gameObject, out playerIndex))
        {
            networkMatchChecker.matchId = _peliID.toGuid();
            TargetjoinGame(true, _peliID, playerIndex);

        }
        else
        {
            Debug.Log("Game could not be hosted");
            TargetjoinGame(false, _peliID, playerIndex);
        }
    }
    [TargetRpc]
    // booli onnistumisesta ja pelin id annetaan tieto ui.lobbylle
    void TargetjoinGame(bool success, string _peliID, int _playerIndex)
    {
        playerIndex = _playerIndex;
        spawnIndex = _playerIndex;
        matchID = _peliID;
        Debug.Log($"MatchId: {matchID} == {_peliID}");
        UILobby.instance.JoinSuccess(success, _peliID);
    }

    /*
    SEARCH GAME
    */
    //Local 
    public void SearchGame()
    {
        CmdSearchGame();
    }
    //kutsuu commandin funktiota
    [Command]
    public void CmdSearchGame()
    {
        if (MatchMaker.instance.SearchGame(gameObject, out playerIndex, out matchID))
        {
            networkMatchChecker.matchId = matchID.toGuid();
            TargetSearchGame(true, matchID, playerIndex);
        }
        else
        {
            Debug.Log($"<color=red>Game not Found</color>");
            TargetSearchGame(false, matchID, playerIndex);
        }

    }
    //joka kutsuu targettia
    [TargetRpc]
    public void TargetSearchGame(bool success, string _peliID, int _playerIndex)
    {
        playerIndex = _playerIndex;
        spawnIndex = _playerIndex;
        matchID = _peliID;
        Debug.Log($"MatchId: {matchID} == {_peliID}");
        UILobby.instance.SearchSuccess(success, _peliID);
    }

    /*
        START GAME
    */
    public void BeginGame()
    {
        CmdBeginGame();
        
        //Pyydetään serveriä toteuttamaan jäädytys
        CmdFreezePlayers();
        CmdDisableLobby();

    }
    [Command]
    void CmdBeginGame()
    {
        MatchMaker.instance.BeginGame(matchID);
        Debug.Log("Game Starting");


    }
    //Pelaaja ajaa tämän kerran
    public void StartGame()
    {
        TargetBeginGame();
    }
    [TargetRpc]
    // booli onnistumisesta ja pelin id annetaan tieto ui.lobbylle
    void TargetBeginGame()
    {
        Debug.Log($"MatchId: {matchID} | Starting");
        //Load game Scene
        SceneManager.LoadScene(2, LoadSceneMode.Additive);
        //StartCoroutine(PlayerSpawnSystem.SpawnPlayer(localPlayer));
        //Käynnistetään pelaajan kamera
        localPlayer.GetComponentInChildren<Camera>().enabled = true;
    }

    /*
    DISCONNECT MATCH
    */

    public void DisconnectGame()
    {
        CmdDisconnectGame();
    }
    [Command]
    void CmdDisconnectGame()
    {
        ServerDisconnect();
    }

    void ServerDisconnect()
    {
        MatchMaker.instance.playerDisconnected(this, matchID);
        // poistetaan matchid käytöstä jotta se voidaan uudelleen osoittaa toiselle pelaajalle tarvittaessa
        networkMatchChecker.matchId = string.Empty.toGuid();
        RpcDisconnectGame();
    }
    [ClientRpc] // clientRpc ilmoittaa kaikille muille käyttäjille että pelaaja on poistunut
    void RpcDisconnectGame()
    {
        ClientDisconnect();
    }
    void ClientDisconnect()
    {
        if (playerLobbyUI != null)
        {
            Destroy(playerLobbyUI);
        }
    }
    /*
    Jäädytys alussa
    */
    [Command]
    private void CmdFreezePlayers()
    {
        //Serveri logiikka jäädytetäänkö ->
        RpcFreezePlayer();
    }
    //serveri logiikan jälkeeen tämä komento toteutetaan kaikilla pelaajilla
    [ClientRpc]
    private void RpcFreezePlayer() => localPlayer.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

    [Command]
    private void CmdDisableLobby()
    {
        RpcDisableLobby();
    }
    [ClientRpc]
    private void RpcDisableLobby()
    {
        Destroy(GameObject.Find("UILobby"), 1f);
    }
    /*
    Kun lähtölaukaus on annettu
    */
    public static void canIbeReleased()
    {
        localPlayer.realeaseMe();
    }

    public void realeaseMe()
    {
        CmdReleaseme();
    }
    [Command]
    private void CmdReleaseme()
    {
        RpcReleaseMyBody();
    }
    [ClientRpc]
    private void RpcReleaseMyBody()
    {
        localPlayer.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        localPlayer.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY;
        localPlayer.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationZ;
    }


}


