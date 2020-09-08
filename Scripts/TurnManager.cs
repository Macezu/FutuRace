using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TurnManager : NetworkBehaviour
{

    private List<PlayerHostScript> pelajaat = new List<PlayerHostScript>();
    
    public void AddPlayer(PlayerHostScript _localplayer)
    {
        pelajaat.Add(_localplayer);
        Debug.Log("Pelaaja on lisätty listalle "+_localplayer.spawnIndex);
    }

}
