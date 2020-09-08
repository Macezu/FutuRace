using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MiniMapScript : MonoBehaviour
{
    public Transform player;


    private void LateUpdate()
    {
        //luodaan vector3 pelaajan sijainnista, haetaan y tieto siitä ja siirretään kamera siihen
        Vector3 newPosition = player.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;
    }
}
