using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class NetworkGameFuturace : NetworkBehaviour
{

    //hook on metodin nimi, silloin kun joku päivitys tapahtuu, eli pelaaja vaihtaa nimeään pyytää serveriä tekemään sen
    //Serveri käyttää hookissa osoitettua metodia päivittämään nimen ja näyttää sen myös muille
    [SyncVar]
    private string displayName = "Loading...";


    //Alempi sitä varten että voidaan puhua jatkossa vain roomista, ettei tarvitse joka kerta kirjoittaa koko pätkää
    private CustomNetworkManager room;
    private CustomNetworkManager Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as CustomNetworkManager;
        }
    }

    //Tämä kutsutaan kaikissa clienteissä
    public override void OnStartClient()
    {
        DontDestroyOnLoad(gameObject);
        //Lisätään pelaaja GamePlayersiin
        Room.GamePlayers.Add(this);
    }

    public override void OnNetworkDestroy()
    {
        //Poistetaan pelaaja kaikilta roomista
        Room.GamePlayers.Remove(this);

    }
    [Server]
    public void SetDisplayName(string _displayname)
    {
        this.displayName = _displayname;
    }

}
