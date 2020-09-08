using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class RoundSystem : NetworkBehaviour
{
    //Erän alkua varten 3,2,1 GO!
    [SerializeField] private Animator animator = null;
    [SerializeField] private inGameUI inGameUI = null;
    private CustomNetworkManager room;

    private CustomNetworkManager Room
    {
        //Haetaan jälleen reference CustomiNetworkManageriin, jos se on olemassa haetaan se,
        //jos ei ole olemassa käydään hakemassa se
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as CustomNetworkManager;
        }
    }

    public void CountdownEnded()
    {
        //Sammuttaa lähtölaskun
        animator.enabled = false;
    }


    #region Server

    public override void OnStartServer()
    {
        //Subscribaus Toimenpiteisiin
        CustomNetworkManager.OnServerStopped += CleanUpServer;
        //Tarkastetaan että voiko aloittaa roundin jokaisen pelaajan kohdalla joka on valmis
        CustomNetworkManager.OnServerReadied += CheckToStartRound;
    }
    //Kun monobehaviour tuhoutuu esim kenttää vaihtaessa => Puhdistetaan serveri;
    [ServerCallback]
    private void OnDestroy() => CleanUpServer();


    private void CleanUpServer()
    {
        CustomNetworkManager.OnServerStopped -= CleanUpServer;
        CustomNetworkManager.OnServerReadied -= CheckToStartRound;
    }

    [ServerCallback]
    public void StartRound()
    {
        
        RpcStartRound();
    }

    private void CheckToStartRound(NetworkConnection conn)
    {
        //Jos isReadyjen määrä on erisuuri kuin pelaajien kokonaismäärä return eli ajetaan vasta kun kaikki valmiita!
        if (Room.GamePlayers.Count(x => x.connectionToClient.isReady) != Room.GamePlayers.Count) { return; }
        inGameUI.arrayOfPlayers = new GameObject[Room.GamePlayers.Count];
        //Aloitetaan lähtölaskenta
        animator.enabled = true;
        //Kaikilla Clienteillä
        RpcStartCountdown();
        inGameUI.arrayOfPlayers = GameObject.FindGameObjectsWithTag("Player");

    }


    #endregion

    #region Client

    [ClientRpc]
    private void RpcStartCountdown()
    {
        animator.enabled = true;
    }

    [ClientRpc]
    private void RpcStartRound()
    {
        VechicleMovement.unFrozen = true;
        inGameUI.startTime = Time.time;
    }

    #endregion
}