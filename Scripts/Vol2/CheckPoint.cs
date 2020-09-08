using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public int checkPointIndex;
    Renderer rend;

    private void Start()
    {
        rend = GetComponent<Renderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<inGameUI>())
        {

            inGameUI iGU = other.GetComponent<inGameUI>();
            if (iGU.currentCheckPoint == checkPointIndex + 1 || iGU.currentCheckPoint == checkPointIndex - 1)
            {
                Vector3 center = rend.bounds.center;
                //Jos ajoneuvon indexi oli yksi suurempi tai pienempi
                iGU.currentCheckPoint = checkPointIndex;
                iGU.checkpointsPassed++;
                iGU.lastCheckPointTrasform = center;
                iGU.RankTheRaces();
            }
        }
    }
}
