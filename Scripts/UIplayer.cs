using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIplayer : MonoBehaviour
{
    // Pelaajan nimeö osoittava teksti
    [SerializeField] TMP_Text text;

    //Referoidaan PlayerHost scriptiä
    PlayerHostScript player;

    public void setPlayer(PlayerHostScript player)
    {
        //Haetaan PlayerhostScriptistä pelaajaref ja sille indexi
        this.player = player;
        text.text = "Player "+player.playerIndex.ToString();
    }
}
