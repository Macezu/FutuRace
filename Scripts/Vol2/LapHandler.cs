using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LapHandler : MonoBehaviour
{
    public int checkpointTotal;

    private void Start()
    {
        //Etsitään kaikki checkpointit
        checkpointTotal = GameObject.FindGameObjectsWithTag("Checkpoint").Length;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<inGameUI>())
        {
            //haetaan toisen pelaajan inGameUi
            inGameUI iGu = other.GetComponent<inGameUI>();

        //Jos pelaajan current checkpoint matchaa totalin kanssa
            if (iGu.currentCheckPoint == checkpointTotal)
            {
                // pelaahan inGameUI skriptissä aloitetaan LapCompleted();

                iGu.LapCompleted();
            }

        }

    }
}