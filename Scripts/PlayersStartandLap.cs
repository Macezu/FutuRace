using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayersStartandLap : MonoBehaviour
{
    public Text Countdown;
    public static bool CountedDown = false;
    float timer = 4f;

    private void Start()
    {
        
        Countdown = gameObject.GetComponentInChildren<Text>();
        Countdown.gameObject.SetActive(false);
        
    }

    private void FixedUpdate()
    {
        if (timer >= 1 && PlayerHostScript.isOnStartupLine)
        {
            Countdown.gameObject.SetActive(true);
            CountdownText();
        } else if (Countdown){
            Countdown.text = "GO!";
        } else {
            return;
        }
        if (CountedDown){
            //Palauttaa Ajoneuvolle liikkumisen ja freeze trasformation poistuu
            PlayerHostScript.canIbeReleased();
            Countdown.text = String.Empty;
        }
        //Anna minulle tietoa siitä monnella sijalla olet pelaaja TargetRpc?

    }

    private void CountdownText()
    {
        timer -= Time.deltaTime;
        float seconds = Mathf.FloorToInt(timer % 60);
        Countdown.text = seconds.ToString();      
        if (seconds == 0){
            CountedDown = true;
        }
    }

    private void liveStandings(){
        //UIssa päivittyy reaaliaikainen pelaajan sijoitus, pyytää serveriltä
    }
}
